using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Library.Extensions;
using Core.Library.Specflow;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Library.WebDriver
{
    public class ProxiedWebDriver : IJavaScriptExecutor, IHasInputDevices
    {
        private static readonly TimeSpan LogWaitTimeAfter = TimeSpan.FromSeconds(1);
        private readonly IHasInputDevices _hasInputDevicesImplementation;
        private readonly IJavaScriptExecutor _javaScriptExecutorImplementation;
        private readonly ScenarioContext _scenarioContext;

        public ProxiedWebDriver(IWebDriver proxiedDriver, ScenarioContext scenarioContext)
        {
            ProxiedDriver = proxiedDriver;
            _javaScriptExecutorImplementation = (IJavaScriptExecutor) proxiedDriver;
            _hasInputDevicesImplementation = (IHasInputDevices) proxiedDriver;
            _scenarioContext = scenarioContext;
        }

        public IWebDriver ProxiedDriver { get; }
        public string Title => ProxiedDriver.Title;
        public string PageSource => ProxiedDriver.PageSource;
        public string CurrentWindowHandle => ProxiedDriver.CurrentWindowHandle;
        public string Url => ProxiedDriver.Url;
        public ReadOnlyCollection<string> WindowHandles => ProxiedDriver.WindowHandles;
        public IOptions Manage => ProxiedDriver.Manage();
        public ProxiedNavigation Navigation => new ProxiedNavigation(ProxiedDriver.Navigate(), _scenarioContext);
        public ITargetLocator SwitchTo => ProxiedDriver.SwitchTo();
        public IKeyboard Keyboard => _hasInputDevicesImplementation.Keyboard;
        public IMouse Mouse => _hasInputDevicesImplementation.Mouse;

        #region Exists

        /// <summary>
        ///     Checks if element exists
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public bool ElementExists(By by)
        {
            return ProxiedDriver.FindElements(by).Any();
        }

        /// <summary>
        ///     Checks is element exists and is visible
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public bool ElementExistsAndVisible(By by)
        {
            return ProxiedDriver.FindElements(by).Any(element => element.Displayed);
        }

        #endregion Exists

        #region Find

        /// <summary>
        ///     Finds an element
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public ProxiedWebElement FindElement(By by)
        {
            TryWaitFor(() => ElementExists(by));

            return new ProxiedWebElement(_scenarioContext, ProxiedDriver.FindElement(by));
        }

        /// <summary>
        ///     Finds a collection of elements
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public ReadOnlyCollection<ProxiedWebElement> FindElements(By by)
        {
            TryWaitFor(() => ElementExists(by));

            var proxiedElements = ProxiedDriver
                .FindElements(by)
                .Select(e => new ProxiedWebElement(_scenarioContext, e))
                .ToArray();

            return new ReadOnlyCollection<ProxiedWebElement>(proxiedElements);
        }

        #endregion Find

        #region Wait

        /// <summary>
        ///     Waits for an element to be visible
        /// </summary>
        /// <param name="by"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool WaitUntilElementVisible(By by, int? timeoutSeconds = null, string message = null)
        {
            return WaitForElementVisibilityState(by, timeoutSeconds ?? ConfigManager.ImplicitWait, true);
        }

        /// <summary>
        ///     Waits for an element to be not visible
        /// </summary>
        /// <param name="by"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public bool WaitUntilElementNotVisible(By by, int? timeoutSeconds = null)
        {
            return WaitForElementVisibilityState(by, timeoutSeconds ?? ConfigManager.ImplicitWait, false);
        }

        public void WaitUntiPageLoaded()
        {
            WaitUntil(driver => ((IJavaScriptExecutor) driver).ExecuteScript("return document.readyState").ToString() ==
                                "complete");
        }

        public void WaitUntiFrameLoaded(string frame)
        {
            WaitUntil(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").ToString() ==
                                "complete");
        }

        /// <summary>
        ///     Keeps trying until the condition is met
        /// </summary>
        /// <param name="by"></param>
        /// <param name="condition"></param>
        /// <param name="description"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public bool WaitUntil(By by, Func<ProxiedWebElement, bool> condition, string description,
            int? timeoutSeconds = null)
        {
            timeoutSeconds = timeoutSeconds ?? ConfigManager.ImplicitWait;

            Func<bool> predicate = () =>
            {
                var element = FindElement(by);
                return condition(element);
            };

            if (!TryWaitFor(predicate, TimeSpan.FromSeconds(timeoutSeconds.Value)))
                throw new NotFoundException(
                    $"Element {by} condition was not met after waiting {timeoutSeconds}s at url: {_scenarioContext.Driver().Url}. {description}");

            return true;
        }

        /// <summary>
        ///     Keeps trying until the condition evaluates true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="failMessage"></param>
        public void WaitUntil(Func<IWebDriver, bool> condition, string failMessage = null)
        {
            try
            {
                new WebDriverWait(ProxiedDriver, ConfigManager.ImplicitWaitTimeSpan)
                    .Until(condition);
            }
            catch (Exception ex)
            {
                failMessage = failMessage ?? "The Driver timed out waiting on a condition with no description";
               throw new TimeoutException(failMessage, ex);
            }
           
        }

        #region Private

        /// <summary>
        ///     polls element until visibility state is as expected or timeout occurs
        /// </summary>
        /// <param name="by"></param>
        /// <param name="timeoutSeconds"></param>
        /// <param name="shouldBeVisible"></param>
        /// <returns></returns>
        private bool WaitForElementVisibilityState(By by, int timeoutSeconds, bool shouldBeVisible = true)
        {
            Func<bool> predicate = () => { return ElementExistsAndVisible(by) == shouldBeVisible; };

            if (!TryWaitFor(predicate, TimeSpan.FromSeconds(timeoutSeconds)))
                throw new NotFoundException(
                    $"Element {by} visibility was: {!shouldBeVisible}, expected: {shouldBeVisible} after waiting {timeoutSeconds}s at url: {_scenarioContext.Driver().Url}");

            return true;
        }

        /// <summary>
        ///     Will continue to execute predicate() for the specified timespan.
        ///     Defaults to AcceptanceTestConfig.ImplicitWaitTimeSpan if not provided
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private bool TryWaitFor(Func<bool> predicate, TimeSpan? timeout = null)
        {
            timeout = timeout ?? ConfigManager.ImplicitWaitTimeSpan;

            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(timeout.Value);
                return TryWait(predicate, cts.Token);
            }
        }

        /// <summary>
        ///     Will continue to execute predicate() until cancellation token expires or an exception is thrown
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private bool TryWait(Func<bool> predicate, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                while (true)
                    try
                    {
                        ct.ThrowIfCancellationRequested();

                        if (predicate()) break;

                        Thread.Sleep(ConfigManager.WaitBetweenChecks);
                    }
                    catch (StaleElementReferenceException)
                    {
                        //do nothing, try again on next attempt
                    }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ConfigManager.AcceptanceTestStatsLoggingEnabled)
                    ProxiedDriver.RecordWait(sw.Elapsed);
            }
        }

        #endregion Private

        #endregion Wait

        #region Retry

        public void Retry(Action doAction, string description, int retryCount = 1, int wait = 0, bool noConsequence = false)
        {
            var numTries = 0;

            while (numTries < retryCount)
            {

                try
                {
                    doAction();
                    return;
                }
                catch (Exception e)
                {
                    numTries++;

                    var message = $"Retry attempt ({numTries}) for '{description}' failed with message: {e}";
                    Console.WriteLine(message);

                    if (numTries >= retryCount)
                    {
                        if (noConsequence) return;
                        throw new Exception(message, e);
                    }

                    Thread.Sleep(wait);
                    
                }
                
            }
            
            
        }

        #endregion

        #region Alert

        public bool IsAlertPresent()
        {
            try
            {
                ProxiedDriver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        public IAlert GetAlert()
        {
            return !IsAlertPresent() ? null : ProxiedDriver.SwitchTo().Alert();
        }

        #endregion

        #region Script Execution

        /// <summary>
        ///     Executes a script with args
        /// </summary>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object ExecuteScript(string script, params object[] args)
        {
            return _javaScriptExecutorImplementation.ExecuteScript(script, args);
        }

        /// <summary>
        ///     Executes a script async
        /// </summary>
        /// <param name="script"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return _javaScriptExecutorImplementation.ExecuteAsyncScript(script, args);
        }

        /// <summary>
        ///     Executes a script with args
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public T ExecuteScript<T>(string script)
        {
            return ProxiedDriver.ExecuteJavaScript<T>(script);
        }

        #endregion Script Execution

        #region Clean Up

        public void Dispose()
        {
            ProxiedDriver.Dispose();
        }

        public void Close()
        {
            ProxiedDriver.Close();
        }

        public void Quit()
        {
            ProxiedDriver.Quit();
        }

        #endregion Clean Up

       
    }
}