using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Library;
using Core.Library.Extensions;
using Core.Library.WebDriver;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Models.Resolver.Forms
{
    public class RequestDetailsModel : BaseModel
    {
        public By VariableMapItems = By.CssSelector("#variable_map item");
        public By CatalogTaskTable = By.Id("sc_req_item.sc_task.request_item_table");
        public By IteneraryAttachmentList = By.Id("itinerary_attachments_select_0");

        private const string _nameMapItemPrefix = "ni.VE";

        public RequestDetailsModel(ScenarioContext context) : base(context)
        {
        }

        #region Verify

        public void VerifyCheckbox(string fieldName, bool isChecked)
        {
            var checkbox = GetTargetByNameMapLabel(fieldName, $"ni.{_nameMapItemPrefix}");
            checkbox.ScrollIntoView();
            Assert.AreEqual(isChecked, checkbox.Selected, $"VerifyCheckbox Failed for field '{fieldName}' expected {isChecked} but found {!checkbox.Selected}");
        }

        public void VerifyText(string fieldName, string expectedValue)
        {
            var textbox = GetTargetByNameMapLabel(fieldName, _nameMapItemPrefix, strategy: NameMapSelectionStrategy.ByIdStartsWith);
            textbox.ScrollIntoView();
            var actualValue = textbox.GetAttribute("value");
            Assert.AreEqual(expectedValue, actualValue, $"VerifyText Failed for field '{fieldName}' expected {expectedValue} but found {actualValue}");
        }

        public void VerifyAutocomplete(string fieldName, string expectedValue)
        {
            var hidden = GetTargetByNameMapLabel(fieldName, _nameMapItemPrefix, shouldBeVisible: false);
            hidden.ScrollIntoView();
            var actualValue = hidden.GetAttribute("value");
            Assert.AreEqual(expectedValue, actualValue, $"VerifyAutocomplete Failed for field '{fieldName}' expected {expectedValue} but found {actualValue}");
        }

        public void VerifySelect(string fieldName, string expectedValue)
        {
            var input = GetTargetByNameMapLabel(fieldName, _nameMapItemPrefix,
                strategy: NameMapSelectionStrategy.ByIdStartsWith);
            input.ScrollIntoView();
            var select = new SelectElement(input);
            
            Assert.AreEqual(expectedValue, select.SelectedOption.Text, $"VerifySelect Failed for field '{fieldName}' expected {expectedValue} but found {select.SelectedOption.Text}");
        }

        public void VerifyGridRowExists(string gridName, string expectedRowData)
        {
            //get the root container of the grid
            var grid = new XGrid.XGrid(ScenarioContext, gridName, (id) => $"container_{_nameMapItemPrefix}{id}");
            
            Assert.IsTrue(grid.VerifyGridData(expectedRowData), $"Could not verify the values exist in the {gridName} grid. Expected {expectedRowData}");
        }

        public void VerifyRadio(string fieldName, string expectedOption)
        {
            //get the radio label
            var label = GetOptionLabelByNameMapAndLabelText(fieldName, expectedOption, idPrefix: _nameMapItemPrefix);
            label.ScrollIntoView();
            //get the input from the label for
            var input = Driver.FindElement(By.Id(label.GetAttribute("for")));
            Assert.IsTrue(input.Selected, $"Radio option {expectedOption} from group {fieldName} was not the selcted option when it was expected to be.");
        }

        public void VerifyDate(string fieldName, int expectedDaysFromNow)
        {
            var textbox = GetTargetByNameMapLabel(fieldName, _nameMapItemPrefix);
            textbox.ScrollIntoView();

            //expect the date in this format: 2018-01-23 11:41:46
            var dateString = textbox.GetAttribute("value");
            DateTime actualDate;
            if(!DateTime.TryParse(dateString, out actualDate))
                throw new ArgumentOutOfRangeException($"Could not parse DateTime from value: '{dateString}'");
            
            //this estimated date should be slightly past the actual date
            var estDate = DateTime.Now.AddDays(expectedDaysFromNow);

            //compare dates with a 6 hr tolerance (account for time differences) - in reality it should be less than 5 mins
            Assert.IsTrue((estDate - actualDate) < TimeSpan.FromHours(6), $"Date comparison was out of acceptable range. Expected: '{estDate}', Actual: '{actualDate}'");
        }

        #endregion

        #region Set

        public void SetRadio(string fieldName, string option)
        {
            //get the radio label
            var label = GetOptionLabelByNameMapAndLabelText(fieldName, option, idPrefix: _nameMapItemPrefix);
            label.ClickTo();
        }

        public void SetText(string fieldName, string value)
        {
            var textbox = GetTargetByNameMapLabel(fieldName, _nameMapItemPrefix, strategy: NameMapSelectionStrategy.ByIdStartsWith);
            textbox.ClickTo();
            textbox.SendKeys(value);
        }

        #endregion

        #region XGrid



        #endregion

        #region Cat Tasks


        /// <summary>
        /// 
        /// </summary>
        /// <param name="catTaskName">Short Description name of the task. case sensitive</param>
        public void OpenTask(string catTaskName)
        {
            //build xpath query to the cat item
            var xpath =
                $"//table[@id='sc_req_item.sc_task.request_item_table']//tr[descendant::td[@title='{catTaskName}']]/td/a[contains(@class, 'formlink')]";
            var link = GetElement(By.XPath(xpath), $"Could not locate Catalog Task link for {catTaskName}");
            link.ClickTo();
        }



        #endregion

        #region Attachments
        
        /// <summary>
        /// Assumes there already exists an attachment on the ticket
        /// This logic will select the first attachment in the Booking details attachment list
        /// and add it to the ticket
        /// </summary>
        public void IncludeAttachment()
        {

            Driver.Retry(() =>
            {
                //get the select
                var selectElement = GetElement(IteneraryAttachmentList);
                var select = new SelectElement(selectElement);
                if (select.Options.Count == 0)
                    throw new NotFoundException("No attachments to include.");
                
                select.SelectByIndex(0);

                //click the add (>) button 
                var addButton =
                    GetElement(By.CssSelector(
                        "div[id='itinerary_attachments_select_0_addRemoveButtons'] .icon-chevron-right"));
                addButton.ClickTo();
            }, "Add Itenerary Attachment", 2, 2000);

        }

        #endregion
    }
}
