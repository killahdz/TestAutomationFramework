using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Core.Library.Extensions
{
    public static class WebDriverExtensions
    {
        /// <summary>
        ///     READ: Failure in correctly clearing cookies between tests can lead to dirty tests
        ///     Please ensure if you are resuing a driver instance to clear the cookies properly
        ///     The framework should take care of this for you but if you change the sequence of
        ///     things in Hooks.cs then you may need to check if this call is being made
        /// </summary>
        /// <param name="driver"></param>
        public static void ClearAndReset(this IWebDriver driver)
        {
            //clear cookies for the current domain first !IMPORTANT!
            driver.Manage().Cookies.DeleteAllCookies();
            //go to reset page
            driver.Navigate().GoToUrl("about:blank");
            //clear cookies again. Note: this call clears cookies only on the about:blank domain whatever that is
            driver.Manage().Cookies.DeleteAllCookies();
        }

        /// <summary>
        ///     Waits for a new window to open
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="expectedNumberOfWindows"></param>
        /// <param name="maxRetryCount"></param>
        public static void WaitUntilNewWindowIsOpened(this IWebDriver driver, Action windowOpenAction,
            int maxRetryCount = 100)
        {
            for (var i = 0; i < maxRetryCount; Thread.Sleep(100), i++)
            {
                windowOpenAction();
                if (driver.WindowHandles.Count > 1) return;
            }

            //try one last time to check for window
            windowOpenAction();

            if (driver.WindowHandles.Count <= 1)
                throw new ApplicationException("New window open not detected.");
        }

        public static bool IsDialogPresent(this IWebDriver driver)
        {
            var alert = ExpectedConditions.AlertIsPresent().Invoke(driver);
            return alert != null;
        }
    }
}