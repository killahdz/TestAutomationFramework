using System;
using Core.Library;
using Core.Library.Helpers;
using Core.Models;
using Core.Models.Portal;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Portal
{
    [Binding]
    public class PortalFormStepDefinitions : BaseStepDefinitions<PortalFormsModel>
    {
        [Given(@"I am on the '(.*)' Form at '(.*)'")]
        public void GivenIAmOnFormTheAtUrl(string formName, string url)
        {
            Model.Navigate(Urls.GetUrl(url), PortalFormsModel.FormLoadedElement, ConfigManager.LongWait);
            Driver.WaitUntiPageLoaded();
        }

        [Then(@"I enter '(.*)' in the '(.*)' text field")]
        public void ThenIEnterSeomthingInTheTextField(string value, string label)
        {
            Model.SetTextboxValueByLabel(label, value);
        }

        [When("I click the form submit button and wait for the page to finish loading")]
        public void WhenISubmitTHeFormAndWaitForLoad()
        {
            Model.Submit();
        }

        [Then(@"I should see the title '(.*)'")]
        public void ThenIShouldSeeTheTitleConfirmingThatMyOrderWasSubmittedSuccessfully(string titleText)
        {
            var title = Driver.FindElement(PortalFormsModel.FormTitle);

            Assert.IsTrue(title.Text.Contains(titleText));
        }

        [Then(@"I check the '(.*)' check box field")]
        public void ThenICheckTheCheckBoxField(string label)
        {
            Model.SetCheckboxValueByLabel(label, true);
        }

        [Then(@"I uncheck the '(.*)' check box field")]
        public void ThenIUncheckTheCheckBoxField(string label)
        {
            Model.SetCheckboxValueByLabel(label, false);
        }


        [Then(@"I select the '(.*)' option in the '(.*)' list")]
        public void ThenISelectTheOptionInTheList(string optionName, string label)
        {
            Model.SelectOptionByLabel(label, optionName);
        }

        [Then(@"In the '(.*)' grid I enter the values '(.*)'")]
        public void ThenInTheGridIEnterTheValues(string label, string values)
        {
            Model.InsertGridValues(label, values);
        }

        [Then(@"In the '(.*)' grid I clear the existing rows")]
        public void ThenInTheGridIClearTheExistingRows(string label)
        {
            Model.ClearGridRows(label);
        }

        [Then(@"In the '(.*)' grid I create a new row and enter the values '(.*)'")]
        public void ThenInTheGridICreateANewRowAndEnterTheValues(string label, string values)
        {
            Model.InsertGridValues(label, values, true);
        }

        [Then(@"I enter '(.*)' random characters into the '(.*)' text field")]
        public void ThenIEnterRandomCharactersIntoTheTextField(int numChars, string label)
        {
            Model.SetTextboxValueByLabel(label, WordGenerator.GenerateSentence(numChars));
        }

        [Then(@"I select the '(.*)' radio option from the '(.*)' field")]
        public void ThenISelectTheRadioOptionFromTheField(string option, string label)
        {
            Model.SelectRadioOptionByLabel(label, option);
        }

        [Then(@"I enter '(.*)' in the '(.*)' autocomplete field and select the first item")]
        public void ThenIEnterInTheAutocompleteFieldAndSelectTheFirstItem(string value, string label)
        {
            Model.SetFirstItemInAutocompleteWithValue(label, value);
        }

        [Then(@"I enter a date \{(.*)} days from now in the '(.*)' date time field with format '(.*)'")]
        public void ThenIEnterADateDaysFromNowInTheDateTimeFieldWithFormat(int daysFromNow, string label, string format)
        {
            Model.SetTextboxValueByLabel(label,
                DateTime.Now.AddDays(daysFromNow).ToString(format ?? PortalFormsModel.DateTimeFormat));
        }

        [Then(@"I enter a date \{(.*)} days from now in the '(.*)' date time field")]
        public void ThenIEnterADateDaysFromNowInTheDateTimeField(int daysFromNow, string label)
        {
            Model.SetTextboxValueByLabel(label,
                DateTime.Now.AddDays(daysFromNow).ToString(PortalFormsModel.DateTimeFormat));
        }



        [Then(@"I enter '(.*)' in the '(.*)' lookup text field")]
        public void ThenIEnterInTheLookupTextField(string value, string label)
        {
            Model.SetTextboxLookupValueByLabel(label, value);
        }

        [Then(@"I add an attachment")]
        public void ThenIAddAnAttachment()
        {
            Model.AddAttachment();
        }

    }
}