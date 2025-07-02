using Core.Library;
using Core.Models;
using Core.Models.Portal;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Portal
{
    [Binding]
    public class PortalStepDefinitions : BaseStepDefinitions<PortalModel>
    {
        [Given(@"I am on the home page")]
        public void GivenIGoToTheHomePage()
        {
            Model.Navigate(Urls.BaseUrl, PortalModel.BodyContentFrame);
        }

        [Then(@"I request for '(.*)'")]
        [When(@"I request for '(.*)'")]
        public void ThenIRequestAs(string username)
        {
            Model.RequestAs(username);
        }

        [Then(@"I should see '(.*)' as the active requestor")]
        public void ThenIShouldSeeAsTheActiveRequestor(string expectedUser)
        {
            Model.CheckRequestor(expectedUser);
        }

        [Then(@"I click the Advanced filter options button")]
        public void ThenIClickTheAdvancedFilterOptionsButton()
        {
            Model.ShowAdvancedFitlerOptions();
        }

        [Then(@"I select the Status All filter option and wait for results")]
        public void ThenISelectTheStatusAllFilterOptionAndWaitForResults()
        {
            Model.ClickFilterOptionStatusAll();
        }

        [Then(@"I verify my Ticket status is '(.*)'")]
        public void ThenIVerifyMyTicketStatusIs(string status)
        {
            Model.VerifyTrackItemStatus(status);
        }

    }
}