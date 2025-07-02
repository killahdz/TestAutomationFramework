using System;
using System.Linq;
using Core.Library;
using Core.Library.Extensions;
using Core.Library.Specflow;
using Core.Library.WebDriver;
using Core.Models.Resolver.Lists;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Models.Resolver.Forms
{
    public class ResolverFormModel : BaseModel
    {
        public static readonly By FormLoadedElement = By.ClassName("form_body");
        public static readonly By SaveButton = By.Id("sysverb_update_and_stay");
        public static readonly By DeleteButton = By.Id("sysverb_delete");
        public static readonly By ApprovedButton = By.Id("approve_approval2_multi");
        public static readonly By RequestApprovalsButton = By.Id("req_multi_approval_wf");
        public static readonly By CompleteTaskButton = By.Id("close_cat_task");
        public static readonly By ResolveButton = By.Id("resolve_incident");
        public static readonly By TabSelector = By.ClassName("tab_caption_text");
        public static readonly By AttachButton = By.Id("header_add_attachment");
        public static readonly By AttachmentItem = By.CssSelector("#attachment_table_body .attachment");
        public static readonly By ModalDialog = By.ClassName("modal-dialog");
        

        public ResolverFormModel(ScenarioContext context) : base(context)
        {
        }

        /// <summary>
        ///     Sets a textbox value using the first item in the suggestion list
        /// </summary>
        /// <param name="val"></param>
        /// <param name="id"></param>
        public void SetTextSuggestion(string val, string id)
        {
            //get the input and set the value
            var input = GetElement(id);
            input.Clear();
            input.SendKeys(val);

            //wait until the lookup list iframe is displayed
            Driver.WaitUntilElementVisible(By.Id($"AC.{input.GetAttribute("data-ref")}_shim"));

            //select the first item
            input.SendKeys(Keys.ArrowDown);
            input.SendKeys(Keys.Enter);
        }

        /// <summary>
        ///     Sets a textbox value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="id"></param>
        public void SetText(string val, string id)
        {
            var input = GetElement(id);
            input.SendKeys(val);
        }


        #region Verify

        /// <summary>
        /// Verifies the input text contains the provided text
        /// </summary>
        /// <param name="val"></param>
        /// <param name="id"></param>
        public void VerifyText(string val, string id)
        {
            var input = GetElement(id);
            Assert.IsTrue(input.Text.Contains(val));
        }

        #endregion Verify

        /// <summary>
        ///     Selects an option from a select list by the text name
        /// </summary>
        /// <param name="optionText"></param>
        /// <param name="id"></param>
        public void SetSelectOption(string optionText, string id)
        {
            SelectElement select = null;
            //wait until the list has items
            //sometimes these are lazy loaded on the setting of another form element
            Driver.WaitUntil((driver) =>
                {
                    select = new SelectElement(GetElement(id));
                    var options = select.Options.ToList();
                    return options.Any(o => o.Text.Equals(optionText));
                }, $"Could not locate option '{optionText}' in select list '{id}'");

            select.SelectByText(optionText);
        }

        /// <summary>
        ///     Sets a checkbox value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="id"></param>
        public void SetCheckbox(bool val, string id)
        {
            var input = GetElement(id);
            if (!input.Selected && val)
                input.Click();
        }


        /// <summary>
        ///     Clicks the Approved button, checks for submission errors and waits for reload
        /// </summary>
        public void ApproveAndWaitForReload()
        {
            ClickButtonAndWaitForReload(ApprovedButton);
        }

        /// <summary>
        ///     Clicks the Request Approvals button, checks for submission errors and waits for reload
        /// </summary>
        public void RequestApprovalsAndWaitForReload()
        {
            ClickButtonAndWaitForReload(RequestApprovalsButton);
        }

        /// <summary>
        ///     Clicks the Save button, checks for submission errors and waits for reload
        ///     For new records only
        /// </summary>
        public void SaveNewAndWaitForReload()
        {
            ClickButtonAndWaitForReload(SaveButton);
        }

        /// <summary>
        ///     Clicks the Complete Task button, checks for submission errors and waits for reload
        /// </summary>
        public void CompleteTaskAndWaitForReload(bool expectAlert = false)
        {
            ClickButtonAndWaitForReload(CompleteTaskButton, expectAlert);
        }

        public void ResolveTicket()
        {
            //click save
            ClickButtonAndWaitForReload(ResolveButton);
        }

        /// <summary>
        ///     Clicks the Save button and checks for script errors
        /// </summary>
        public void SaveAndCheckSubmitErrors()
        {
            //click save
            GetElement(SaveButton).Click();

            ShortWait(2000);

            CheckForSubmissionErrors();
        }

        

        private void ClickButtonAndWaitForReload(By button, bool expectAlert = false)
        {
            //click save
            GetElement(button).Click();

            //handle unexpected alerts
            if (expectAlert)
            {
                Driver.Retry(() =>
                {
                    Driver.GetAlert().Accept();
                }, "ClickButtonAndWaitForReload: Accept alert", 3, 1000, true);
            }

            CheckForSubmissionErrors();
            CheckForSupressedFieldErrors();

            //wait for reload
            ShortWait(2000);
            Driver.WaitUntiPageLoaded();
        }

        /// <summary>
        ///     Assigns a value to a field via the lookup popup window
        /// </summary>
        /// <param name="lookupStr"></param>
        /// <param name="id"></param>
        internal void Lookup(string lookupStr, string id)
        {
            if (ConfigManager.WebDriverType == WebDriverType.ie)
            {
                SetTextSuggestion(lookupStr, id);
                return;
            }

            //Get the lookup button via xpath from the input
            var lookupButton = GetElement(By.XPath($"//input[@id='{id}']/./..//a[@role='button']"));
            if (lookupButton == null)
                throw new NotImplementedException($"Could not locate lookup Button for '{id}'");

            //wait for new window - especially IE
            Driver.ProxiedDriver.WaitUntilNewWindowIsOpened(() =>
            {
                DismissNotifications();
                lookupButton.Click();
            });

            //switch context to the popup window and wait for it to load
            SwitchToLastWindow();

            try
            {
                Driver.WaitUntilElementVisible(BaseListModel.BodyContentDetail);
            }
            catch (NotFoundException nfex)
            {
                //log the page source
                Driver.RecordLog(nfex.Message);
                Driver.RecordLog(Driver.PageSource);
                throw;
            }

            //find the search text box
            var searchInput =
                Driver.FindElement(By.XPath($"//span[@id='sys_user_hide_search']//input[@placeholder='Search']"));
            if (searchInput == null)
                throw new NotImplementedException($"Could not locate Search Input");

            //set the value into the search box and hit enter
            searchInput.SendKeys(lookupStr);
            searchInput.SendKeys(Keys.Enter);

            //wait until the column filters appear to know the page has reloaded
            Driver.WaitUntilElementVisible(
                By.CssSelector($".breadcrumb_container a[filter='GOTOnameLIKE{lookupStr}']"));

            //search the rows and get the first item
            var rows = Driver.FindElements(BaseListModel.ListRows);
            if (!rows.Any())
                throw new NotImplementedException($"No results displayed for search text {lookupStr}");
            var row = rows.First();

            //look in the row for all anchors and select the first one with the name starting with the search term
            var anchor = row.FindElements(By.TagName("a"))
                .FirstOrDefault(a => a.Text.ToLower().StartsWith(lookupStr.ToLower()));

            //click the link to fill the value in the main window
            if (anchor == null)
                throw new NotImplementedException($"Could not locate link");
            anchor.Click();

            //switch back context
            SwitchToMainWindow();
        }

        /// <summary>
        /// Clicks the specified tab
        /// </summary>
        /// <param name="tabText"></param>
        public void ClickTab(string tabText)
        {
            var tabs = Driver.FindElements(TabSelector);
            if (tabs == null || !tabs.Any())
                throw new NotFoundException("Could not locate any tabs");

            var tab = tabs.FirstOrDefault(t => t.Text.StartsWith(tabText));
            if(tab == null)
                throw new NotFoundException($"Could not locate the {tabText} tab");

            tab.ClickTo();
        }


        public void VerifyAttachmentExists()
        {
            //click the attachment button and wait for the dialog
            GetElement(AttachButton).Click();

            //wait til the modal is visible
            Driver.WaitUntilElementVisible(ModalDialog);

            //get an attachment element
            GetElement(AttachmentItem, "Could not detect an attachment on the form when expecting one");

            //click the body to remove the dialog
            GetElement(By.TagName("body")).Click();

        }

       
    }
}