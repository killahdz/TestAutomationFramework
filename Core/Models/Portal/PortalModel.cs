using System;
using System.Linq;
using System.Windows.Forms;
using Core.Library;
using Core.Library.Extensions;
using Core.Library.WebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Models.Portal
{
    public class PortalModel : BaseModel
    {
        internal By HomePageContentTitle = By.ClassName("portal-contentHeaderTitle");
        internal string HomePageUrl = "rttms";

        public static By LoadingIndicator = By.ClassName("rtp_loading_title");
        internal By RequestForEveryoneTab = By.CssSelector("#tab-everyone span");
        internal By RequestForLink = By.CssSelector(".show-dialog-requestFor.as-link");
        internal By RequestForSelectUserButton = By.ClassName("c1lose-user-dialog");
        internal By RequestForUserResults = By.ClassName("selectUser");
        internal By RequestForUserSearchTextbox = By.Id("userSearch");
        internal By AdvancedFilterOptionsButton = By.ClassName("rtp-button-search-advanced");
        internal By HideAdvancedFilterOptionsButton = By.ClassName("rtp-slide-close");
        internal By AdvancedFilterOptionsSidepanel = By.ClassName("rtp-slide-container");
        internal By AdvancedFilterOptionsStatusAllButton = By.XPath("//div[contains(@class, 'rtp_search-filters-section')]/h5[contains(text(), 'Status')]/following-sibling::div[contains(@class, 'mdl-grid')]//span[contains(@class, 'mdl-chip__text')][text() = 'All']/following-sibling::button");
        internal By TrackItemStatusBadge = By.CssSelector("h4 ticket-status-drop-detail .status-badge");

        public static readonly By BodyContentFrame = By.Id("gsft_main");

        public PortalModel(ScenarioContext context) : base(context)
        {
        }

        /// <summary>
        ///     Requests as another user
        /// </summary>
        /// <param name="username"></param>
        internal void RequestAs(string username)
        {

            FindAndClick(RequestForLink);
            FindAndClick(RequestForEveryoneTab);

            var textBox = FindAndClick(RequestForUserSearchTextbox);
            textBox.SendKeys(username);
            //wait for results
            var results = Driver.FindElements(RequestForUserResults);
            if (results == null || !results.Any())
                throw new NotFoundException($"Could not detect Request for user results for search term '{username}'");

            if (results.Count == 1)
                results[0].Click();
            else
                results[1].Click();

            ShortWait(1000);

            //record the username for verfication
            //var intendedUser = Driver.ExecuteScript<string>($"return jQuery('#userSearch').val();");

            Func<ProxiedWebElement, bool> predicate = (element) => { return element.Enabled && element.Displayed; };

            //wait until select user button is enabled
            Driver.WaitUntil(RequestForSelectUserButton, predicate, "Element enabled and displayed");

            FindAndClick(RequestForSelectUserButton);

            //wait for loading
            Driver.WaitUntilElementVisible(LoadingIndicator);
            Driver.WaitUntilElementNotVisible(LoadingIndicator);
        }

        internal void CheckRequestor(string expectedUser)
        {
            //verify we are requesting for the right user
            var actualUser = Driver.ExecuteScript<string>("return jQuery('.show-dialog-requestFor.as-link').text();");
            Assert.AreEqual(expectedUser, actualUser, "Failed to Request for User");
        }

        public void ShowAdvancedFitlerOptions()
        {
            //click the button
            GetElement(AdvancedFilterOptionsButton).Click();
            //wait until filter panel is visible
            ShortWait(1000);
            Driver.WaitUntilElementVisible(AdvancedFilterOptionsSidepanel);
        }

        public void HideAdvancedFilterOptions()
        {
            GetElement(HideAdvancedFilterOptionsButton).Click();
            ShortWait(1000);
        }

        public void ClickFilterOptionStatusAll()
        {
            GetElement(AdvancedFilterOptionsStatusAllButton).Click();
            Driver.WaitUntilElementVisible(LoadingIndicator);
            Driver.WaitUntilElementNotVisible(LoadingIndicator, ConfigManager.LongWait);
            ShortWait(1000);
            HideAdvancedFilterOptions();
        }

        public void ClickItemInTrackSearchResults(string ticketNumber)
        {
            //use xpath 
            var xpath = $"//ul[contains(@class, 'mdl-list')]/li[contains(span, '{ticketNumber}')]/span/span/a";
            var link = GetElement(By.XPath(xpath), $"Could not locate Ticket link for {ticketNumber} with xpath {xpath}");
            link.ClickTo();

            ShortWait(1000);
            Driver.WaitUntiPageLoaded();
        }

        public void VerifyTrackItemStatus(string expectedStatus)
        {
            Driver.WaitUntiPageLoaded();

            Driver.WaitUntil((driver) =>
            {
                var statusElement = GetElement(TrackItemStatusBadge, "Could not locate Track Item status link");
                return statusElement.Text.Equals(expectedStatus, StringComparison.InvariantCultureIgnoreCase);
            }, $"Could not confirm Track Item Status was '{expectedStatus}' ");
            
        }
    }
}