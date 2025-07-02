using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Library;
using Core.Models;
using OpenQA.Selenium;
using Core.StepDefinitions;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions
{
    [Binding]
    public class EndToEndStepDefinitions : BaseStepDefinitions<EndToEndModel>
    {

        [Then(@"I remember my Unique Ticket Number")]
        public void ThenIRememberMyUniqueTicketNumber()
        {
            Model.RecordTicket();
        }

        [Then(@"I search the Resolver list '(.*)' by '(.*)' for my Unique Ticket Number")]
        public void ThenISearchTheListByForMyUniqueTicketNumber(string searchContainer, string searchBy)
        {
            Model.SearchForTicketResolver(searchContainer, searchBy);
        }

        [Then(@"I search Track for my Unique Ticket Number")]
        public void ThenISearchTrackForMyUniqueTicketNumber()
        {
            //Model.TicketNumber = "RITM1517092";//TODO: delete
            Model.SearchyForTicketPortalTrack();
        }

        [Then(@"I click the search result link for my Unique Ticket Number")]
        public void ThenIClickTheSearchResultLinkForMyUniqueTicketNumber()
        {
            Model.ClickItemInTrackSearchResults(Model.TicketNumber);
        }
        

    }
}
