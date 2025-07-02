using Core.Models.Resolver.Lists;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions.Resolver
{
    [Binding]
    public class ListStepDefinitions : BaseStepDefinitions<BaseListModel>
    {
        [Then(@"I go to the '(.*)' List at '(.*)'")]
        [Given(@"I am on the '(.*)' List at '(.*)'")]
        public void ThenIGoToTheList(string title, string url)
        {
            Model.GoToList(url);
        }

        [Then(@"I open a random list item and wait for the element with class '(.*)'")]
        public void ThenIOpenARandomListItemAndWaitForTheElementWithClass(string className)
        {
            Model.OpenRandomRow();
            Driver.WaitUntilElementVisible(By.ClassName(className));
        }


        [Then(@"I open the first list item")]
        public void ThenIOpenTheFirstListItem()
        {
            Model.OpenFirstRow();
        }
    }
}