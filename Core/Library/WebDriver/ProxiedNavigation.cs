using System;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Library.WebDriver
{
    public class ProxiedNavigation : INavigation
    {
        private readonly ScenarioContext _scenarioContext;

        public ProxiedNavigation(INavigation navigation, ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            Navigation = navigation;
        }

        public INavigation Navigation { get; }

        public void Back()
        {
            Navigation.Back();
        }

        public void Forward()
        {
            Navigation.Forward();
        }

        public void Refresh()
        {
            Navigation.Refresh();
        }

        public void GoToUrl(string url)
        {
            GoToUrl(new Uri(url));
        }

        public void GoToUrl(Uri url)
        {
            Navigation.GoToUrl(url);
        }
    }
}