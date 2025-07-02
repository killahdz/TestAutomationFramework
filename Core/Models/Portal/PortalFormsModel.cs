using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Library;
using Core.Library.Extensions;
using Core.Library.Mapping;
using Core.Library.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Models.Portal
{
    public class PortalFormsModel : PortalModel
    {
        internal static readonly By FormLoadedElement = By.ClassName("rt-page-content-main");
        internal static readonly By FormSubmitButton = By.Id("orderDetail_submit");
        internal static readonly By FormTitle = By.ClassName("oc-title");
        internal static readonly By ProgressSpinner = By.ClassName("mdl-spinner__circle");
        internal static readonly By OrderSummary = By.ClassName("oc");
        internal static readonly By AutomcompleteSuggestions = By.ClassName("autocomplete-suggestions");
        internal static readonly By LookupSuggestions = By.ClassName("ac_dropdown");
        internal static readonly string FormElementTooltipSelector = ".sn-tooltip-basic";


        internal static readonly string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        private static readonly string FormFrameId = "gsft_main";

        public PortalFormsModel(ScenarioContext context) : base(context)
        {
        }
        
        public IWebElement ActiveElement => Driver.SwitchTo.ActiveElement();
        
        #region Submit

        /// <summary>
        ///     Submits the form by clicking the Submit button in the parent frame
        ///     and waits for the order summary to show
        /// </summary>
        public void Submit()
        {
            //switch back to the parent frame
            SwitchToDefaultFrame();

            //locate the submit button element
            var button = Driver.FindElement(FormSubmitButton);
            if (button == null)
                throw new ApplicationException($"Could not locate Form Submit Button '{FormSubmitButton.ToString()}'");

            //click the submit button
            button.ClickTo();

            //wait 2 seconds
            ShortWait(2000);

            //check for submit errors
            CheckForSubmissionErrors();

            //switch back to form frame
            SwitchToFormFrame();

            //wait for order summary to show
            Driver.WaitUntilElementVisible(OrderSummary);
        }

        #endregion Submit
        
        #region Get

        /// <summary>
        ///     Gets target input by its name map label
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="idPrefix"></param>
        /// <param name="idTransformFunc"></param>
        /// <returns></returns>
        internal ProxiedWebElement GetTargetByNameMapLabel(string nameMapLabel, string idPrefix = null,
            Func<string, string> idTransformFunc = null)
        {
            return base.GetTargetByNameMapLabel(nameMapLabel, idPrefix, idTransformFunc, FormFrameId);
        }

      

        /// <summary>
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="labelText"></param>
        /// <returns></returns>
        private ProxiedWebElement GetOptionLabelByNameMapAndLabelText(string nameMapLabel, string labelText)
        {
            return base.GetOptionLabelByNameMapAndLabelText(nameMapLabel, labelText, FormFrameId);
        }

        internal void AddAttachment()
        {
            //click the manage attachment link and wait
            var manageAttachLink = Driver.FindElement(By.Id("header_attachment_list_link"));
            manageAttachLink.ClickTo();

            var attahInputBy = By.Id("attachFile");
            Driver.WaitUntilElementVisible(attahInputBy);

            var attahInput = Driver.FindElement(attahInputBy);
            attahInput.SendKeys(Paths.AttachmentPath);

            //close the modal
            var attachButton = Driver.FindElement(By.Id("attachButton"));
            attachButton.ClickTo();

            Driver.WaitUntilElementNotVisible(By.Id("please_wait"));

            //close the modal
            var body = Driver.FindElement(By.TagName("body"));
            //todo: null checks
            body.Click();
        }

        #region Grid

        internal void ClearGridRows(string label)
        {
            //switch to form frame
            SwitchToFormFrame();

            //get the root container of the grid
            var grid = new XGrid.XGrid(ScenarioContext, label);

            grid.RemoveRows();
        }

        /// <summary>
        ///     Puts the supplied values in the xGrid     
        /// </summary>
        /// <param name="label">grid label</param>
        /// <param name="values">delimited values</param>
        /// <param name="addNewRow">Clicking the add button?</param>
        internal void InsertGridValues(string label, string values, bool addNewRow = false)
        {
            //switch to form frame
            SwitchToFormFrame();

            //get the root container of the grid
            var grid = new XGrid.XGrid(ScenarioContext, label);

            if (addNewRow)
                grid.AddNewRow(values);
            else
                grid.AddToExistingRow(values);
        }

        #endregion Grid

        /// <summary>
        ///     Gets a textbox by its nameMap label
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <returns></returns>
        private ProxiedWebElement GetCheckboxByLabel(string nameMapLabel)
        {
            return GetTargetByNameMapLabel(nameMapLabel, "ni.");
        }

        #endregion Get

        #region Set

        /// <summary>
        /// Enters text then waits for autocomplete suggestions and selects the first item
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="value"></param>
        internal void SetFirstItemInAutocompleteWithValue(string nameMapLabel, string value)
        {
            //the autocomplete IDs are not the typical form: IO:0e35ea97dbd4f200c702f8fdae9619a0
            //they take this form: input_0e35ea97dbd4f200c702f8fdae9619a0
            //this func does that transform
            try
            {
                Func<string, string> transformFunc = (id) => { return $"input_{id.Replace("IO:", "")}"; };

                var autocomplete = GetTargetByNameMapLabel(nameMapLabel, idTransformFunc: transformFunc);

                //retry this twice, suspect sendkeys is getting sent before javascript is listening for autocomplete
                Driver.Retry(() =>
                {
                    autocomplete.Clear();
                    autocomplete.SendKeysTo(value);
                    Driver.WaitUntilElementVisible(AutomcompleteSuggestions, ConfigManager.ShortWait);
                }, $"set value '{value}' on Autocomplete '{nameMapLabel}'", 2, ConfigManager.MicroWait);

                autocomplete.SendKeys(Keys.ArrowDown);
                autocomplete.SendKeys(Keys.Enter);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set value '{value}' on Autocomplete '{nameMapLabel}'", e);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="optionLabelText"></param>
        /// <param name="reassure"></param>
        public void SelectRadioOptionByLabel(string nameMapLabel, string optionLabelText, bool reassure = true)
        {
            
            Driver.Retry(() =>
            {
                //sometimes tooltips get in the way
                HideTooltips();

                //when form elements are expanding while a click is sent to a coord, sometimes the driver will
                //miss click. Retry to account for it
                GetOptionLabelByNameMapAndLabelText(nameMapLabel, optionLabelText).ClickTo();
            },
            $"Failed to set option '{optionLabelText}' on Radio '{nameMapLabel}'", 2, ConfigManager.MicroWait);
        }

        /// <summary>
        ///     Sets the value of an input[type=text]
        /// </summary>
        /// <param name="nameMapLabel">name map label</param>
        /// <param name="value"></param>
        public void SetTextboxValueByLabel(string nameMapLabel, string value)
        {
            try
            {
                //Get the input
                var input = GetTargetByNameMapLabel(nameMapLabel);

                DismissNotifications();

                //clear and set
                input.SendKeysTo(value, true);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set value '{value}' on Textbox '{nameMapLabel}'", e);
            }


        }

        /// <summary>
        ///     Sets value on a lookup control
        ///     Waits until suggestion is displayed
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="value"></param>
        internal void SetTextboxLookupValueByLabel(string nameMapLabel, string value)
        {
            try
            {
                Func<string, string> transformFunc = (id) => { return id.Replace("IO:", "sys_display.IO:"); };

                var input = GetTargetByNameMapLabel(nameMapLabel, idTransformFunc: transformFunc);
                input.SendKeysTo(value);

                //wait for the lookup suggestions
                Driver.WaitUntilElementVisible(LookupSuggestions, message: $"Timed out waiting for Lookup suggestions on form element '{nameMapLabel}' with value '{value}'");
                CurrentFocusProxy.SendKeys(Keys.ArrowDown).Perform();
                CurrentFocusProxy.SendKeys(Keys.Tab).Perform();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set value '{value}' on Textbox Lookup '{nameMapLabel}'", e);
            }

        }

        /// <summary>
        ///     Clicks a checkbox
        /// </summary>
        /// <param name="nameMapLabel"></param>
        /// <param name="value"></param>
        internal void SetCheckboxValueByLabel(string nameMapLabel, bool value)
        {
            try
            {
                //get the checkbox
                var checkbox = GetCheckboxByLabel(nameMapLabel);

                //set the value
                if (value && !checkbox.Selected || !value && checkbox.Selected)
                    checkbox.SendKeysTo(Keys.Space);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set value '{value}' on Checkbox '{nameMapLabel}'", e);
            }

        }

        internal void SelectOptionByLabel(string nameMapLabel, string optionName)
        {
            try
            {
                //get the select list
                var select = new SelectElement(GetTargetByNameMapLabel(nameMapLabel));

                //items could still be loading - wait until the option is in the list
                Driver.WaitUntil((driver) => { return select.Options.Any(o => o.Text.Equals(optionName)); }, 
                    $"Failed to locate option {optionName} in select list {nameMapLabel}");

                //set it
                select.SelectByText(optionName);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to set option '{optionName}' on select list '{nameMapLabel}'", e);
            }

        }

        #endregion Set

        #region Frames

        public void SwitchToFormFrame()
        {
            SwitchFrame(FormFrameId);
        }

        public void SwitchToDefaultFrame()
        {
            try
            {
                //switch back to the parent frame
                Driver.SwitchTo.DefaultContent();
            }
            catch (NoSuchFrameException)
            {
                //webdriver throws an exception when attempting to switch frames
                //when you are already on the target frame
                //there is no simple mechanism to check the current frame so we handle it this way
            }
        }

        #endregion Frames

        #region Misc

        public void HideTooltips()
        {
            HideElementByScript(FormElementTooltipSelector);
        }

        #endregion
    }
}