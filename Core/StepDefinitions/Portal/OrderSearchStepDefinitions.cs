using Core.Library;
using Core.Models;
using Core.Models.Portal.Pages;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Portal
{
    [Binding]
    public class OrderSearchStepDefinitions : BaseStepDefinitions<OrderSearchPageModel>
    {
        [Given(@"I go to the Portal Order page and wait for it to load")]
        [Then(@"I go to the Portal Order page and wait for it to load")]
        public void GivenIGoToTheOrderPage()
        {
            Model.Navigate(Urls.GetUrl(Model.OrderSearchPageUrl), Model.OrderSearchPageLoadedElement);
        }

        [Then(@"I type '(.*)' in the Search text box")]
        public void GivenITypeInTheSearchTextBox(string searchTerm)
        {
            Driver.FindElement(Model.OrderSearchInput)
                .SendKeys(searchTerm);
        }

        [Given(@"I wait for search results to appear")]
        [When(@"I wait for search results to appear")]
        [Then(@"I wait for search results to appear")]
        public void GivenIWaitForSearchResultsToAppear()
        {
            Model.OrderSearchWaitForSearchResults();
        }

        [When(@"I click the search result with title '(.*)' and wait for the form to load")]
        public void WhenIClickTheSearchResultWithTitle(string title)
        {
            Model.OrderSearchSelectForm(title);
        }
    }
}