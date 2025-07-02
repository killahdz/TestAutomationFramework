using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Models.Portal.Pages
{
    public class OrderSearchPageModel : PortalModel
    {
        public OrderSearchPageModel(ScenarioContext context) : base(context)
        {
        }

        #region Order Search

        internal string OrderSearchPageUrl = "rttms/#/order/search";

        internal By OrderSearchResultsListContainer = By.Id("list-panel");
        internal By OrderSearchResultTitle = By.ClassName("mdl-list__item-text-title");

        internal By OrderSearchInput = By.Id("term");
        internal By OrderSearchPageLoadedElement => By.ClassName("mdl-button--primary");

        #endregion Order Search

        #region Order Search

        public void OrderSearchSelectForm(string title)
        {
            //get all the visible results
            var resultsContainer = Driver.FindElement(OrderSearchResultsListContainer);
            Assert.IsNotNull(resultsContainer,
                "Could no locate Search results container: " + OrderSearchResultsListContainer.ToString());

            //search for our result
            var theResult = Driver.FindElements(OrderSearchResultTitle)
                .FirstOrDefault(e => e.Text == title);
            Assert.IsNotNull(theResult, "Could not find search result with title: " + title);

            //click it
            theResult.Click();

            //wait for the form to load
            Driver.WaitUntilElementVisible(By.Id("gsft_main"), 60);
        }

        internal void OrderSearchWaitForSearchResults()
        {
            //wait for loading indicator to display
            Driver.WaitUntilElementVisible(LoadingIndicator);
            //wait for it to go away
            Driver.WaitUntilElementNotVisible(LoadingIndicator);

            //check the results
            var resultsContainer = Driver.FindElement(OrderSearchResultsListContainer);
            Assert.IsNotNull(resultsContainer,
                "Could no locate Search results container: " + OrderSearchResultsListContainer.ToString());

            var resultCount = resultsContainer.FindElements(By.TagName("ul")).Count;
            Assert.GreaterOrEqual(resultCount, 1, "No Search results found");
        }

        #endregion Order Search
    }
}