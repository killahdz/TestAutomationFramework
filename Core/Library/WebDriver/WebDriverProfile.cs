using Core.Library.Extensions;
using Core.Library.Helpers;

namespace Core.Library.WebDriver
{
    public enum WebDriverType
    {
        [BrowserProcess("chrome")] [WebDriverProcess("chromedriver")]
        chrome,

        [BrowserProcess("iexplore")] [WebDriverProcess("iedriverserver")]
        ie,

        //Firefox,
        //PhantomJs
    }

    public class WebDriverProfile
    {
        private readonly ProcessHelper _browserProcessHelper;
        private readonly ProcessHelper _driverProcessHelper;

        public WebDriverProfile(bool isMobileMode, WebDriverType webDriverType)
        {
            IsMobileMode = isMobileMode;
            WebDriverType = webDriverType;
            _browserProcessHelper = new ProcessHelper(BrowserProcessName);
            _driverProcessHelper = new ProcessHelper(DriverProcessName);
        }

        public bool IsMobileMode { get; }

        public string DriverProcessName => WebDriverType.GetEnumAttribute<WebDriverProcessAttribute>().Name;

        public string BrowserProcessName => WebDriverType.GetEnumAttribute<BrowserProcessAttribute>().Name;

        public WebDriverType WebDriverType { get; }

        public override int GetHashCode()
        {
            return (int) WebDriverType * (IsMobileMode ? -1 : 1);
        }

        public override bool Equals(object obj)
        {
            var other = obj as WebDriverProfile;
            if (other == null) return false;
            return other.IsMobileMode == IsMobileMode && other.WebDriverType == WebDriverType;
        }

        /// <summary>
        ///     Kills driver and browser processes
        /// </summary>
        public void CleanUpProcesses()
        {
            //kill all driver processes
            _driverProcessHelper.Kill();

#if DEBUG
            //dont close all the browsers
            _browserProcessHelper.Kill(false);
#else
//kill only spawned browser processes
            BrowserProcessHelper.Kill();
#endif
        }
    }
}