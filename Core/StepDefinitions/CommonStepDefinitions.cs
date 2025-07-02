using System.Threading;
using Core.Library;
using Core.Library.Exceptions;
using Core.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions
{
    [Binding]
    public class CommonStepDefinitions : BaseStepDefinitions<BaseModel>
    {
        [Then(@"I should observe no script errors on the page")]
        public void ThenThereAreNoScriptErrorsOnThePage()
        {
            try
            {
                //Check console logs
                Model.CheckForConsoleErrors();
                Model.CheckForSupressedFieldErrors();
            }
            catch (ScriptErrorException scex)
            {
                Assert.Fail(scex.ToString());
            }
        }

        [Given(@"I wait for (.*) second")]
        [Given(@"I wait for (.*) seconds")]
        [When(@"I wait for (.*) second")]
        [When(@"I wait for (.*) seconds")]
        [Then(@"I wait for (.*) seconds")]
        public void WhenIWaitForSecond(int waitTimeInSeconds)
        {
            Model.ShortWait(waitTimeInSeconds * 1000);
        }
        

        [Given(@"I go to this url '(.*)' and wait until I see the element with this class '(.*)'")]
        public void GivenIAmAtThisUrl(string url, string cssClass)
        {
            Model.Navigate(Urls.GetUrl(url), By.ClassName(cssClass), ConfigManager.LongWait);
        }

        [Given(@"I go to this url '(.*)' and wait until I see the element with the selector '(.*)'")]
        public void GivenIAmAtThisUrlBySelector(string url, string selector)
        {
            Model.Navigate(Urls.GetUrl(url), By.CssSelector(selector));
        }
        
        [Then(@"I dismiss notifications")]
        public void ThenIDismissNotifications()
        {
            Driver.ExecuteScript("jQuery('.notification').hide();");
        }

        [Then(@"I remember the current Url for the '(.*)' page")]
        public void ThenIRememberTheCurrentUrlForThePage(string urlKey)
        {
            Model.SaveValue(urlKey, Driver.Url);
        }

        [Then(@"I go to the Url I remembered for the '(.*)' page")]
        [Then(@"I return to the Url I remembered for the '(.*)' page")]
        public void ThenIGoToTheUrlIRememberedForThePage(string urlKey)
        {
            Driver.Navigation.GoToUrl(Model.GetSavedValue(urlKey));
            Driver.WaitUntiPageLoaded();
        }

    }
}