using System;
using Core.Library;
using Core.Library.WebDriver;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Models
{
    public class LoginModel : BaseModel
    {
        public string HideToolTipScript = "jQuery('.tooltip-inner').hide();";
        public By ImpersonateMenuItem = By.CssSelector("a[sn-modal-show='impersonate']");
        public By UnimpersonateMenuItem = By.XPath("//ul[contains(@class, 'dropdown-menu')]//a[text()='Unimpersonate']");
        public By ImpersonateSearch = By.Id("select2-chosen-2");
        public By ImpersontateResult = By.ClassName("select2-result");
        public By LoginPagePasswordTextbox = By.Id("user_password");
        public By LoginPageUsernameTextbox = By.Id("user_name");
        public string LoginUrl = "login.do";
        public string LogoutUrl = "logout.do";
        public By UserInfoMenu = By.CssSelector("ul.dropdown-menu[aria-labelledby='user_info_dropdown']");
        public By UserInfoMenuButton = By.Id("user_info_dropdown");

        public LoginModel(ScenarioContext context) : base(context)
        {
        }

        public void Login(string username, string password)
        {
            //Assumes focus is on the username textbox on page load
            CurrentFocusProxy.SendKeys(username).Perform();
            CurrentFocusProxy.SendKeys(Keys.Tab).Perform();
            CurrentFocusProxy.SendKeys(password).Perform();
            CurrentFocusProxy.SendKeys(Keys.Enter).Perform();

            Driver.WaitUntilElementVisible(UserInfoMenuButton);
        }

        public void Logout()
        {
            Driver.Navigation.GoToUrl(Urls.GetUrl(LogoutUrl));
            ShortWait(2000);
            Driver.Navigation.GoToUrl(Urls.GetUrl(LoginUrl));
        }

        public void Impersonate(string user)
        {
            //click the menu button
            FindAndClick(UserInfoMenuButton);
            //locate the menu
            var menu = Driver.FindElement(UserInfoMenu);

            Action<ProxiedWebElement> beforeImpersonateClickActions = (element) =>
            {
                if (ConfigManager.DriverTypeStrong == WebDriverType.chrome)
                    CurrentFocusProxy.MoveToElement(element).Perform();
                Driver.ExecuteScript(HideToolTipScript);
                ShortWait(1000);
            };
            
            //click impersonate menu link
            FindAndClick(ImpersonateMenuItem, menu.FindElement, beforeImpersonateClickActions);
           
            //click impersonate search
            FindAndClick(ImpersonateSearch);
            
            //enter text
            CurrentFocusProxy.SendKeys(user).Perform();
            //wait for results
            Driver.WaitUntilElementVisible(ImpersontateResult);
            //hit enter
            CurrentFocusProxy.SendKeys(Keys.Enter).Perform();
            
            ShortWait(1000);

            //wait for page to reload
            Driver.WaitUntiPageLoaded();
        }

        public void Unimpersonate()
        {
            //click the menu button
            FindAndClick(UserInfoMenuButton);

            //locate the menu
            var menu = Driver.FindElement(UserInfoMenu);

            Driver.WaitUntilElementVisible(UnimpersonateMenuItem,
                message: $"Could not locate Unimpersonate menu item by {UnimpersonateMenuItem.ToString()}");

            Action<ProxiedWebElement> beforeImpersonateClickActions = (element) =>
            {
            
                if (ConfigManager.DriverTypeStrong == WebDriverType.chrome)
                    CurrentFocusProxy.MoveToElement(element).Perform();
                Driver.ExecuteScript(HideToolTipScript);
                ShortWait(1000);
            };
            
            //click impersonate menu link
            FindAndClick(UnimpersonateMenuItem, menu.FindElement, beforeImpersonateClickActions);

            ShortWait(1000);

            //wait for page to reload
            Driver.WaitUntiPageLoaded();
        }
    }
}