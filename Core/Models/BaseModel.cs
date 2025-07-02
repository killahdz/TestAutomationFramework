using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Library;
using Core.Library.Exceptions;
using Core.Library.Extensions;
using Core.Library.Mapping;
using Core.Library.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Models
{
    public class BaseModel
    {
        public By PageTimingDiv => By.Id("page_timing_div");
        public ScenarioContext ScenarioContext;

        public BaseModel(ScenarioContext context)
        {
            ScenarioContext = context;
        }

        public ProxiedWebDriver Driver => ScenarioContext?.Driver();
        public Actions CurrentFocusProxy => new Actions(Driver.ProxiedDriver);
        public string MainWindowHandle => Driver.WindowHandles.First();
        public string LastWindowHandle => Driver.WindowHandles.Last();
        
       private Dictionary<string, string> _savedValues = new Dictionary<string, string>();

        #region Navigation

        public  void Browse(ScenarioContext scenarioContext, By expectedElement, string url, int pageTimeout = 0)
        {
            var driver = scenarioContext.Driver();
            if (url != null)
                driver.Navigation.GoToUrl(url);

            driver.WaitUntilElementVisible(expectedElement, pageTimeout > 0 ? pageTimeout : ConfigManager.ImplicitWait);
        }

        public void Navigate(string url, By cssSelector, int? timeout = null)
        {
            Browse(ScenarioContext, cssSelector, url, timeout.GetValueOrDefault());
        }

        #endregion

        #region NameMap

        public enum NameMapSelectionStrategy
        {
            //Exact match on ID
            ByIdExact = 0,
            //Id begins with - translates to [id^='value']
            ByIdStartsWith = 1
        }

        internal List<NameMap> NameMap { get; private set; }

        /// <summary>
        ///     Gets the name map via javascript and stores it in class var NameMap
        /// </summary>
        internal void GetNameMap(string frame = null)
        {
            var testScript = "return g_form != null;";
            var getScript = "return g_form.nameMap;";

            //switch scope to iframe
            if (frame != null)
                SwitchFrame(frame);

            var js = Driver as IJavaScriptExecutor;
            if (js == null) return;

            //execute js
            //dynamic scriptResult = js.ExecuteScript($"return g_form.nameMap;");
            var maps = new List<NameMap>();

            var scriptWait = new WebDriverWait(Driver.ProxiedDriver, TimeSpan.FromSeconds(90));
            var scriptTryCount = 0;
            dynamic nameMaps = scriptWait.Until(x =>
            {
                try
                {
                    var gForm = Driver.ProxiedDriver.ExecuteJavaScript<bool>(testScript);
                    if (gForm) return Driver.ProxiedDriver.ExecuteJavaScript<IEnumerable>(getScript);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"g_form not available after {++scriptTryCount} attempts: {ex.Message}");
                }

                return null;
            });

            //parse the result
            foreach (var item in nameMaps)
                maps.Add(new NameMap
                {
                    RealName = item["realName"],
                    PrettyName = item["prettyName"]
                });

            NameMap = maps;
        }

        /// <summary>
        ///     Gets target input by its name map label
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="idPrefix"></param>
        /// <param name="idTransformFunc"></param>
        /// <param name="frameId">Frame to execute in</param>
        /// <param name="strategy">Determines how the selector is used to locate the element in the DOM.
        /// Check the comments on the enum values for more detail</param>
        /// <param name="shouldBeVisible"></param>
        /// <returns></returns>
        internal ProxiedWebElement GetTargetByNameMapLabel(
            string nameMapLabel, 
            string idPrefix = null,
            Func<string, string> idTransformFunc = null, 
            string frameId = null,
            NameMapSelectionStrategy strategy = NameMapSelectionStrategy.ByIdExact,
            bool shouldBeVisible = true)
        {
            //this must be called when the page has been loaded
            if(NameMap == null)
                GetNameMap(frameId);

            //switch to frame
            if(frameId != null)
                SwitchFrame(frameId);

            //locate the item in the map
            var item = NameMap.FirstOrDefault(nm => nm.PrettyName.Equals(nameMapLabel));
            if (item == null)
                throw new ApplicationException($"Item not found '{nameMapLabel}' in name map");

            var itemId = $"{idPrefix}{item.RealName}";

            //if we need to transform the id for special cases do it here
            if (idTransformFunc != null)
                itemId = idTransformFunc(itemId);

            //get the item from the dom
            ProxiedWebElement element = null;

            if (strategy == NameMapSelectionStrategy.ByIdStartsWith)
                element = Driver.FindElement(By.CssSelector($"[id^='{itemId}']"));
            else
                element = Driver.FindElement(By.Id(itemId));

            if (element == null)
                throw new ApplicationException($"Input not found '{item.PrettyName}':'{itemId}' in DOM");

            //wait until visible before attempting to interact
            if (shouldBeVisible)
                Driver.WaitUntilElementVisible(By.Id(itemId));

            return element;
        }

        /// <summary>
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="labelText"></param>
        /// <param name="frameId"></param>
        /// <param name="idPrefix"></param>
        /// <returns></returns>
        internal ProxiedWebElement GetOptionLabelByNameMapAndLabelText(
            string nameMapLabel, 
            string labelText, 
            string frameId = null,
            string idPrefix = null)
        {
            //this must be called when the page has been loaded
            if (NameMap == null)
                GetNameMap(frameId);

            //switch to frame
            if (frameId != null)
                SwitchFrame(frameId);

            //locate the item in the map
            var item = NameMap.FirstOrDefault(nm => nm.PrettyName.Equals(nameMapLabel));
            if (item == null)
                throw new ApplicationException($"Item not found '{nameMapLabel}' in name map");

            var itemId = $"{idPrefix}{item.RealName}";

            //get the label by the text and the for
            var labelXPath = $"//label[text()='{labelText}'][starts-with(@for, '{itemId}')]";

            //identify the label for our option by the label text
            var label = Driver.FindElement(By.XPath(labelXPath));
            if (label == null)
                throw new ApplicationException($"Label '{labelText}' not found for group: '{itemId}'");

            //wait until visible before attempting to interact
            Driver.WaitUntilElementVisible(By.XPath(labelXPath));

            return label;
        }

        #endregion

        public void SwitchFrame(string frameId)
        {
            try
            {
                //switch to form frame
                Driver.SwitchTo.Frame(frameId);
            }
            catch (NoSuchFrameException)
            {
                //webdriver throws an exception when attempting to switch frames
                //when you are already on the target frame
                //there is no simple mechanism to check the current frame so we handle it this way
            }
        }

        public virtual void ShortWait(int ms = 50)
        {
            Thread.Sleep(ms);
        }

        internal void SwitchToLastWindow()
        {
            Driver.SwitchTo.Window(LastWindowHandle);
        }

        internal void SwitchToMainWindow()
        {
            Driver.SwitchTo.Window(MainWindowHandle);
        }

        internal virtual ProxiedWebElement GetElement(string id)
        {
            return GetElement(By.Id(id));
        }

        internal ProxiedWebElement GetElement(By by, string message = null)
        {
            var element = Driver.FindElement(by);
            if (element == null)
                throw new NotImplementedException(message ?? $"Could not find element with id '{by.ToString()}'");

            element.ScrollIntoView();
            return element;
        }

        #region Notifications

        public void DismissNotifications()
        {
            try
            {
                //dont dismiss errors
                HideElementByScript(".notification:not(.notification-error)");
            }
            catch (Exception)
            {
                //TODO: Log
            }
        }

        public void HideElementByScript(string selector)
        {
            Driver.ExecuteScript($"jQuery('{selector}').hide();");
        }

        #endregion Notifications

        #region Find

        /// <summary>
        ///     Finds an element, performs actions before clicking then clicks
        /// </summary>
        /// <param name="elementToFind">The element to find</param>
        /// <param name="findAction">method to find the element, specifying null will search the dom</param>
        /// <param name="beforeClickActions">actions to perform before attempting to click the element</param>
        /// <returns></returns>
        internal ProxiedWebElement FindAndClick(By elementToFind, Func<By, ProxiedWebElement> findAction = null,
            Action<ProxiedWebElement> beforeClickActions = null)
        {
            //find the target element either from the root of the DOM or from the method provided
            var element = findAction == null ? Driver.FindElement(elementToFind) : findAction(elementToFind);
            if (element == null)
                throw new NotFoundException($"Could not locate element {element}");

            //perform some actions if provided
            beforeClickActions?.Invoke(element);

            //click and scroll to the element
            element.ClickTo();

            //return it
            return element;
        }

        #endregion Find

        public void SetContext(ScenarioContext context)
        {
            ScenarioContext = context;
        }

        #region Error Checking

        /// <summary>
        ///     Checks the browser logs for errors
        /// </summary>
        public void CheckForConsoleErrors()
        {
            if (!WebDriverProvider.Settings.BrowserLogs)
                return;

            var errorDetectionString = "A script has encountered an error in render events";

            var logs = Driver.Manage.Logs.GetLog(LogType.Browser).ToList();

            //iterate through the logs looking for our error detection string
            //the next item in the list should be the actual error detail
            var errorDetectedPrev = false;

            var errorLogs = new List<LogEntry>();

            foreach (var log in logs)
            {
                //detect severe errors raised from the browser
                //ignore errors on the exclude list - false positives
                if (log.Level == LogLevel.Severe && !ScriptErrors.CanIgnore(log.Message))
                    errorLogs.Add(log);

                //this is a suppressed error that was detected previously
                if (errorDetectedPrev)
                {
                    errorLogs.Add(log);
                    errorDetectedPrev = false;
                    continue;
                }

                //some errors are suppressed and output to the console as Info
                //detect the first line of output and record the next line as this is the actual error detail
                if (log.Message.ToLower().Contains(errorDetectionString.ToLower()))
                    errorDetectedPrev = true;
            }

            if (errorLogs.Any())
                throw new ScriptErrorException(errorLogs);
        }

        public void CheckForSupressedFieldErrors()
        {
            var errorDetectionString = "script error";
            try
            {
                var errors = Driver.ExecuteScript<string>("return jQuery('.notification-error').text();");
                if (errors.ToLower().Contains(errorDetectionString))
                    throw new ScriptErrorException(errors, ScriptErrorScope.Field);
            }
            catch (ScriptErrorException)
            {
                throw;
            }
            catch (Exception)
            {
                //TODO: Log
            }
        }

        public void CheckForSubmissionErrors()
        {
            var errorDetectionString = "script error";
            try
            {
                var errors =
                    Driver.ExecuteScript<string>(
                        "return jQuery('.outputmsg_error.notification-error .outputmsg_text').text();");
                if (errors.ToLower().Contains(errorDetectionString))
                    throw new ScriptErrorException(errors, ScriptErrorScope.Form);
            }
            catch (ScriptErrorException)
            {
                throw;
            }
            catch (Exception)
            {
                //TODO: Log
            }
        }

        #endregion Error Checking

        #region Saved Values
        
        public void SaveValue(string key, string value)
        {
            if(!_savedValues.ContainsKey(key))
                _savedValues.Add(key, value);
            else
                _savedValues[key] = value;
        }

        public string GetSavedValue(string key)
        {
            return _savedValues[key];
        }

        #endregion
    }
}