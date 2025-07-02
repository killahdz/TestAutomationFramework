using System;
using OpenQA.Selenium;

namespace Core.Library.WebDriver
{
    /// <summary>
    ///     Helper class to store "create driver" configuration
    ///     Maps a WebDriverType to a specified method for driver creation
    /// </summary>
    public class WebDriverProviderMap
    {
        public WebDriverProviderMap(WebDriverType webDriverType, Func<IWebDriver> func)
        {
            WebDriverType = webDriverType;
            Func = func;
        }

        public WebDriverType WebDriverType { get; set; }
        public Func<IWebDriver> Func { get; set; }
    }
}