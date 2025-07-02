using System;
using System.Configuration;
using Core.Library.Extensions;
using Core.Library.WebDriver;

namespace Core.Library
{
    public static class ConfigManager
    {
        #region Do we need to implement??

        public static bool CheckJavascriptErrors => RetrieveValue("CheckJavascriptErrors").ToUpper().Contains("Y");

        #endregion Do we need to implement??

        /// <summary>
        ///     Website root url
        ///     This is the website we want to test
        /// </summary>
        public static string WebsiteRoot => RetrieveValue("WebsiteRoot");

        /// <summary>
        ///     Browser - eg. 'firefox', 'ie', 'chrome'
        /// </summary>
        public static string DriverType => RetrieveValue("Browser");

        /// <summary>
        ///     Browser - eg. 'firefox', 'ie', 'chrome'
        /// </summary>
        public static WebDriverType DriverTypeStrong =>
            (WebDriverType) Enum.Parse(typeof(WebDriverType), RetrieveValue("Browser"));

        /// <summary>
        ///     This gets set via a build step in TeamCity
        /// </summary>
        public static string BuildNumber =>
            Environment.GetEnvironmentVariable($"BUILDNUMBER_{DriverType}_{EnvTag}".ToUpper(),
                EnvironmentVariableTarget.User) ?? "0";

        /// <summary>
        ///     Driver type based on app config setting
        /// </summary>
        public static WebDriverType WebDriverType
        {
            get
            {
                switch (DriverType.ToLower())
                {
                    case "ie":
                        return WebDriverType.ie;

                    case "chrome":
                    default:
                        return WebDriverType.chrome;
                }
            }
        }

        /// <summary>
        ///     PATH to chrome executable
        /// </summary>
        public static string ChromeExe => RetrieveValue("ChromeExe");

        /// <summary>
        ///     PATH to chrome executable
        /// </summary>
        public static string EnvTag => RetrieveValue("EnvTag");

        /// <summary>
        ///     PATH to chrome executable
        /// </summary>
        public static bool UseWebDriverVideoCapture =>
            RetrieveValue("UseWebDriverVideoCapture").ToUpper().Contains("Y");

        /// <summary>
        ///     Retrieves a value from firstly the system ENV variable, then Config Manager
        /// </summary>
        /// <param name="key">The name of the environment variable</param>
        /// <returns></returns>
        private static string RetrieveValue(string key)
        {
            return Environment.GetEnvironmentVariable("AppSetting_" + key) ??
                   Environment.GetEnvironmentVariable(key) ??
                   ConfigurationManager.AppSettings[key];
        }

        #region Supports

        /// <summary>
        ///     Setting for Browser console logs
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static bool SupportsBrowserLogs => DriverTypeStrong.IsOneOf(WebDriverType.chrome);

        /// <summary>
        ///     Setting for screenshot capabaility
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static bool SupportsScreenshots => DriverTypeStrong.IsOneOf(WebDriverType.chrome);

        /// <summary>
        ///     Setting for screenshot capabaility
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static bool SupportsVideo => DriverTypeStrong.IsOneOf(WebDriverType.chrome);

        #endregion Supports

        #region Test stats Logging

        /// <summary>
        ///     Enable logging
        /// </summary>
        public static bool AcceptanceTestStatsLoggingEnabled =>
            RetrieveValue("AcceptanceTestStatsLoggingEnabled").ToUpper().Contains("Y");

        public static string AcceptanceTestStatsConnectionString =>
            RetrieveValue("AcceptanceTestStatsDb") ?? string.Empty;

        #endregion Test stats Logging

        #region Waits

        public static int ImplicitWait => int.TryParse(RetrieveValue("ImplicitWait"), out var wait) ? wait : 30;

        public static TimeSpan ImplicitWaitTimeSpan => TimeSpan.FromSeconds(ImplicitWait);

        public static int LongWait => int.TryParse(RetrieveValue("LongWait"), out var wait) ? wait : 60;

        public static TimeSpan LongWaitTimeSpan => TimeSpan.FromSeconds(LongWait);

        public static int ShortWait => int.TryParse(RetrieveValue("ShortWait"), out var wait) ? wait : 15;

        public static int MicroWait => int.TryParse(RetrieveValue("MicroWait"), out var wait) ? wait : 3;

        public static int WaitBetweenChecks =>
            int.TryParse(RetrieveValue("WaitBetweenChecks"), out var wait) ? wait : 500;

        public static bool PreserveVideoLogs => RetrieveValue("PreserveVideoLogs") == "true";

        #endregion Waits
    }
}