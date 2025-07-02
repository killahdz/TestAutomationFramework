using Core.Library;
using Core.Models;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions : BaseStepDefinitions<LoginModel>
    {
        [Given(@"I am on the Login Form")]
        public void GivenIAmOnTheLoginForm()
        {
            Model.Navigate(Urls.GetUrl(Model.LoginUrl), Model.LoginPageUsernameTextbox);
        }

        [Then(@"I login with the username '(.*)' and password '(.*)'")]
        [When(@"I login with the username '(.*)' and password '(.*)'")]
        public void ThenILoginWithTheUsernameAndPassword(string username, string password)
        {
            Model.Login(username, password);
        }

        [Then(@"I Should be Logged in")]
        public void ThenIShouldBeLoggedIn()
        {
            Assert.IsTrue(Driver.ElementExists(Model.UserInfoMenuButton),
                $"Could not verify user is logged in. Expected to find {Model.UserInfoMenuButton}");
        }

        [Then(@"I Logout")]
        public void ThenILogout()
        {
            Model.Logout();
        }
        
        [Then(@"I impersonate as '(.*)'")]
        public void ThenIImpersonateAs(string impersonateUser)
        {
            Model.Impersonate(impersonateUser);
        }

        [Then(@"I unimpersonate")]
        public void ThenIUnimpersonate()
        {
            Model.Unimpersonate();
        }

    }
}