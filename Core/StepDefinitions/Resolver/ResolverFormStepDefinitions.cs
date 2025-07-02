using Core.Library;
using Core.Models;
using Core.Models.Resolver.Forms;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Resolver
{
    [Binding]
    public class ResolverFormStepDefinitions : BaseStepDefinitions<ResolverFormModel>
    {
        [Then(@"I click the Save button and check for submit errors")]
        public void ThenIClickTheSaveButtonAndCheckForSubmitErrors()
        {
            Model.SaveAndCheckSubmitErrors();
        }

        [Then(@"I click the Save button and wait for the form to reload")]
        public void ThenIClickTheSaveButtonAndWaitForTheFormToReload()
        {
            Model.SaveNewAndWaitForReload();
        }

        [Then(@"I go to the '(.*)' Resolver Form at '(.*)'")]
        public void ThenIGoToTheResolverFormAt(string formName, string url)
        {
            Model.Navigate(Urls.GetUrl(url), ResolverFormModel.FormLoadedElement);
        }

        [Then(@"I enter the text '(.*)' into the '(.*)' resolver text field")]
        public void ThenIEnterTheTextIntoTheResolverTextField(string text, string id)
        {
            Model.SetText(text, id);
        }

        [Then(@"I verify the text '(.*)' exists in the '(.*)' resolver text field")]
        public void ThenIVerifyTheTextExistsInTheResolverTextField(string text, string id)
        {
            Model.VerifyText(text, id);
        }

        [Then(@"I enter the text '(.*)' into the '(.*)' resolver suggestion text field")]
        public void ThenIEnterTheTextIntoTheResolverSuggestionTextField(string text, string id)
        {
            Model.SetTextSuggestion(text, id);
        }

        [Then(@"I select the '(.*)' option in the '(.*)' resolver select field")]
        public void ThenISelectTheOptionInTheResolverSelectField(string option, string id)
        {
            Model.SetSelectOption(option, id);
        }

        [Then(@"I check the '(.*)' label for the resolver checkbox field")]
        public void ThenICheckTheResolverCheckboxField(string id)
        {
            Model.SetCheckbox(true, id);
        }

        [Then(@"I uncheck the '(.*)' resolver checkbox field")]
        public void ThenIUncheckTheResolverCheckboxField(string id)
        {
            Model.SetCheckbox(false, id);
        }

        [Then(@"I Lookup '(.*)' and select the first matching result into the '(.*)' resolver text field")]
        public void ThenILookupAndSelectTheFirstMatchingResultIntoTheResolverTextField(string value, string id)
        {
            Model.Lookup(value, id);
        }

        [Then(@"I click the '(.*)' tab")]
        public void ThenIClickTheTab(string tabText)
        {
            Model.ClickTab(tabText);
        }

        [Then(@"I click the Resolve Ticket button and check for form submission errors")]
        public void ThenIClickTheResolveTicketButtonAndCheckForFormSubmissionErrors()
        {
            Model.ResolveTicket();
        }

        [Then(@"I verify that an attachment exists on the resolver form")]
        public void ThenIVerifyThatAnAttachmentExistsOnTheResolverForm()
        {
            Model.VerifyAttachmentExists();
        }

        [Then(@"I click the Complete Task button and wait for the form to reload")]
        public void ThenIClickTheCompleteTaskButtonAndWaitForTheFormToReload()
        {
            Model.CompleteTaskAndWaitForReload();
        }

        [Then(@"I click the Complete Task button, accept alerts and wait for the form to reload")]
        public void ThenIClickTheCompleteTaskButtonAndAcceptAlertsWaitForTheFormToReload()
        {
            Model.CompleteTaskAndWaitForReload(true);
        }

        [Then(@"I click the Approved button and wait for the form to reload")]
        public void ThenIClickTheApprovedButtonAndWaitForTheFormToReload()
        {
            Model.ApproveAndWaitForReload();
        }

        [Then(@"I click the Request Approvals button and wait for the form to reload")]
        public void ThenIClickTheRequestApprovalsButtonAndWaitForTheFormToReload()
        {
            Model.RequestApprovalsAndWaitForReload();
        }

    }
}