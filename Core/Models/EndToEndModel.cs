using System;
using Core.Library;
using Core.Models.Portal;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Core.Models
{
    public class EndToEndModel : PortalModel
    {
        public By SubmittedTicketLink =
            By.XPath("//div[contains(@class, 'rttms-order-summary')]//span[span[text()='Reference Number']]//a");


        public By PortalTrackSearchTextbox = By.Id("term");


        public EndToEndModel(ScenarioContext context) : base(context)
        {
        }

        private const string TicketNumberKey = "TicketNumber";
        public string TicketNumber
        {
            get => GetSavedValue(TicketNumberKey);
            set => SaveValue(TicketNumberKey, value);
        }

        public void RecordTicket(string val = null)
        {
            if (val == null)
            {
                var ticketAnchor = Driver.FindElement(SubmittedTicketLink);
                if (ticketAnchor == null)
                    throw new NotFoundException("Could not locate ticket number link");
                TicketNumber= ticketAnchor.Text;
            }
            else
            {
                TicketNumber = val;
            }

            Assert.IsNotEmpty(TicketNumber, "Could not locate submitted Ticket number");
            Console.WriteLine($"Created new Ticket: {TicketNumber}");
        }

        public void SearchForTicketResolver(string searchContainer, string searchBy)
        {
            Driver.WaitUntiPageLoaded();
            var resolverListSearchByList = By.CssSelector($"#{searchContainer}_hide_search select");

            //get the select list and set the search by
            var searchByList = new SelectElement(GetElement(resolverListSearchByList));
            searchByList.SelectByText(searchBy);

            ShortWait(1000);
            //set the search term
            CurrentFocusProxy.SendKeys(TicketNumber).Perform();
            CurrentFocusProxy.SendKeys(Keys.Enter).Perform();
            
            ShortWait(1000);
            Driver.WaitUntiPageLoaded();
        }

        /// <summary>
        /// Searches for the saved ticket on the portal track form
        /// </summary>
        public void SearchyForTicketPortalTrack()
        {
            //get the search textbox
            var searchBox = GetElement(PortalTrackSearchTextbox, "Could not locate Track Search textbox");
            searchBox.SendKeys(TicketNumber);
            Driver.WaitUntilElementVisible(PortalModel.LoadingIndicator);
            Driver.WaitUntilElementNotVisible(PortalModel.LoadingIndicator, ConfigManager.LongWait);
        }

        
    }
}