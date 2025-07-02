using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Library.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using TechTalk.SpecFlow;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Core.Library.WebDriver
{
    /// <summary>
    ///     Handles the creation and clean up of driver instances
    /// </summary>
    public class WebDriverProvider
    {
        private static readonly TimeSpan WebDriverConnectionTimeout = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan ScriptTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ImplicitlyWaitTimeout = TimeSpan.Zero;
        private static readonly TimeSpan PageloadTimeout = new TimeSpan(0, 0, 5, 0);
        private static readonly Size WindowSize = new Size(1440, 900);

        private static readonly ThreadSafeDictionary<WebDriverProfile, Pool<IWebDriver>> DriverPool =
            new ThreadSafeDictionary<WebDriverProfile, Pool<IWebDriver>>();

        private static readonly List<WebDriverProviderMap> DriverFactoryMap
            = new List<WebDriverProviderMap>
            {
                new WebDriverProviderMap(WebDriverType.chrome, CreateChromeDriver),
                //new WebDriverProviderMap(WebDriverType.PhantomJs, CreatePhantomDriver),
                new WebDriverProviderMap(WebDriverType.ie, CreateIeDriver),
                //new WebDriverProviderMap(WebDriverType.Firefox, CreateFirefoxDriver)
            };

        private readonly ScenarioContext _scenarioContext;

        public WebDriverProvider(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            //this.logger = scenarioContext.Logger();
        }

        /// <summary>
        ///     Returns a web driver in a ready state for testing
        /// </summary>
        /// <param name="profile">Specify </param>
        /// <returns></returns>
        public PooledItem<IWebDriver> Borrow(WebDriverProfile profile, bool reset = false)
        {
            Action<IWebDriver> resetAction = (drv) =>
            {
                if (reset) ResetDriver(drv);
            };

            //Retrieve the driver from the pool
            var driver = DriverPool.GetOrAdd(profile, p => new Pool<IWebDriver>(() => CreateDriver(p), resetAction))
                .BorrowItem();
            //logger.Debug("WebDriverProvider.Get: IsMobileMode {IsMobileMode} WebDriverType {WebDriverType}", profile.IsMobileMode, profile.WebDriverType);

            return driver;
        }

        /// <summary>
        ///     Returns the driver to the pool
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="driver"></param>
        public void Return(WebDriverProfile profile, PooledItem<IWebDriver> driver)
        {
            var pool = DriverPool.FirstOrDefault(dp => dp.Key.WebDriverType == profile.WebDriverType);
            pool.Value.ReturnItem(driver.Value);
        }
        //private ILogger logger;

        public static class Settings
        {
            public static bool TakeScreenShots = true;
            public static bool BrowserLogs = true;
        }

        #region Driver Creation Methods

        /// <summary>
        ///     Pass in a profile and this will make you the appropriate web driver
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        private static IWebDriver CreateDriver(WebDriverProfile profile)
        {
            //Execute the correct factory method for this driver type
            var driver = DriverFactoryMap
                .First(m => m.WebDriverType == profile.WebDriverType)
                .Func();

            //configure timeouts
            if (profile.WebDriverType.IsNotOneOf(WebDriverType.chrome)) //, WebDriverType.PhantomJs))
                driver.Manage().Timeouts().PageLoad = PageloadTimeout;

            driver.Manage().Timeouts().ImplicitWait = ImplicitlyWaitTimeout;
            //driver.Manage().Window.Maximize();
            //configure globals
            Settings.BrowserLogs = ConfigManager.SupportsBrowserLogs;
            Settings.TakeScreenShots = ConfigManager.SupportsScreenshots;

            return driver;
        }

        /// <summary>
        ///     Creates an Internet Explorer driver
        /// </summary>
        /// <returns></returns>
        private static IWebDriver CreateIeDriver()
        {
            var ieOptions = new InternetExplorerOptions
            {
                //Must be set to True else sendKeys will duplicate characters
                EnableNativeEvents = true,

                RequireWindowFocus = false,
                EnablePersistentHover = true,
                // IntroduceInstabilityByIgnoringProtectedModeSettings = true//experimental
            };
            ieOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);

            var driver = new InternetExplorerDriver(ieOptions);

            //Log.Debug("Setting up web driver to use {Browser} with options {@BrowserOptions}", "IE", null);
            return driver;
        }

        /// <summary>
        ///     Creates a chrome driver instance
        /// </summary>
        /// <returns></returns>
        private static IWebDriver CreateChromeDriver()
        {
            var chromeOptions = new ChromeOptions
            {
                BinaryLocation = Paths.Browsers.Chrome,
            };
            //chromeOptions.AddArguments("user-data-dir=C:\\Users\\Daniel.Kereama\\AppData\\Local\\Google\\Chrome\\User Data\\Default");
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);
            chromeOptions.UnhandledPromptBehavior = UnhandledPromptBehavior.Dismiss;
            //chromeOptions.AddArgument("no-sandbox");
            chromeOptions.AddArgument("--start-fullscreen");

            //Log.Debug("Setting up web driver to use {Browser} with options {@BrowserOptions}", "Chrome", chromeOptions);
            return new ChromeDriver(chromeOptions);
        }

        /// <summary>
        ///     Creates a PhantomJs Driver
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///     Creates a Firefox Driver
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///     resets the driver to stating position
        ///     Clears cookies and navigates to blank page
        /// </summary>
        /// <param name="driver"></param>
        public static void ResetDriver(IWebDriver driver)
        {
            driver.ClearAndReset();
        }

        /// <summary>
        ///     Clean Up all the things
        /// </summary>
        public static void CleanUp()
        {
            //define actions to dispose the driver
            Action<IWebDriver> driverDisposeAction = (d) =>
            {
                d.Quit(); //chromedriver needs to execute this to kill the process
                d.Dispose();
            };

            //do it
            DriverPool
                .Do(pool =>
                {
                    //dispose the pool
                    pool.Value.Dispose(driverDisposeAction);

                    //kill the drivers and related browsers
                    pool.Key.CleanUpProcesses();
                })
                .Done();
        }

        #endregion Clean up
    }
}