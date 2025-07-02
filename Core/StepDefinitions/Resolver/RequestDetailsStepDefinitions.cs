using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Resolver.Forms;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Resolver
{
    [Binding]
    public class RequestDetailsStepDefinitions : BaseStepDefinitions<RequestDetailsModel>
    {
        [Then(@"I verify the '(.*)' request details check box field is checked")]
        public void ThenIVerifyTheRequestDetailsCheckBoxFieldIsChecked(string fieldName)
        {
            Model.VerifyCheckbox(fieldName, true);
        }

        [Then(@"I verify the '(.*)' request details check box field is unchecked")]
        public void ThenIVerifyTheRequestDetailsCheckBoxFieldIsUnChecked(string fieldName)
        {
            Model.VerifyCheckbox(fieldName, false);
        }

        [Then(@"I verify '(.*)' exists in the '(.*)' request details text field")]
        public void ThenIVerifyExistsInTheRequestDetailsTextField(string expectedValue, string fieldName)
        {
            Model.VerifyText(fieldName, expectedValue);
        }

        [Then(@"I verify '(.*)' exists in the '(.*)' request details autocomplete field")]
        public void ThenIVerifyExistsInTheRequestDetailsAutocompleteField(string expectedValue, string fieldName)
        {
            Model.VerifyAutocomplete(fieldName, expectedValue);
        }


        [Then(@"I verify the '(.*)' option is selected in the '(.*)' request details list")]
        public void ThenIVerifyTheOptionIsSelectedInTheRequestDetailsList(string expectedValue, string fieldName)
        {
            Model.VerifySelect(fieldName, expectedValue);
        }

        [Then(@"I Verify the '(.*)' grid contains the row '(.*)'")]
        public void ThenIVerifyTheGridContainsTheRow(string gridName, string rowValues)
        {
            Model.VerifyGridRowExists(gridName, rowValues);
        }


        [Then(@"I verify the '(.*)' option is selected from the '(.*)' request details radio field")]
        public void ThenIVerifyTheOptionIsSelectedFromTheRequestDetailsRadioField(string expectedOption, string fieldName)
        {
            Model.VerifyRadio(fieldName, expectedOption);
        }

        [Then(@"I verify the date is approximately \{(.*)} days from now in the '(.*)' request details date time field")]
        public void ThenIVerifyTheDateIsApproximatelyDaysFromNowInTheRequestDetailsDateTimeField(int expectedDaysFromNow, string fieldName)
        {
            Model.VerifyDate(fieldName, expectedDaysFromNow);
        }

        [Then(@"I open the '(.*)' Catalog Task")]
        public void ThenIOpenTheCatalogTask(string taskName)
        {
            Model.OpenTask(taskName);
        }

        [Then(@"I set the option '(.*)' on the '(.*)' request details radio group")]
        public void ThenISetTheOptionOnTheRequestDetailsRadioGroup(string optionName, string fieldName)
        {
            Model.SetRadio(fieldName, optionName);
        }

        [Then(@"I set the value '(.*)' on the '(.*)' request details textbox")]
        public void ThenISetTheValueOnTheRequestDetailsTextbox(string value, string fieldName)
        {
            Model.SetText(fieldName, value);
        }

        [Then(@"I include the first Itenerary attachment")]
        public void ThenIIncludeTheFirstIteneraryAttachment()
        {
            Model.IncludeAttachment();
        }


    }
}
