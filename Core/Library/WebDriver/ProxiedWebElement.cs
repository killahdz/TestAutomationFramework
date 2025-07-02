using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Core.Library.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using TechTalk.SpecFlow;

namespace Core.Library.WebDriver
{
    public class ProxiedWebElement : IWebElement, ILocatable
    {
        private readonly ILocatable _locatableImplementation;

        public ProxiedWebElement(ScenarioContext scenarioContext, IWebElement proxiedElement)
        {
            ScenarioContext = scenarioContext;
            ProxiedElement = proxiedElement;
            _locatableImplementation = (ILocatable) proxiedElement;
        }

        public IWebElement ProxiedElement { get; }
        public ScenarioContext ScenarioContext { get; }

        Point ILocatable.LocationOnScreenOnceScrolledIntoView =>
            _locatableImplementation.LocationOnScreenOnceScrolledIntoView;

        ICoordinates ILocatable.Coordinates => _locatableImplementation.Coordinates;

        IWebElement ISearchContext.FindElement(By by)
        {
            return FindElement(by);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            var downcastElements = FindElements(by)
                .Cast<IWebElement>()
                .ToArray();
            return new ReadOnlyCollection<IWebElement>(downcastElements);
        }

        public void Clear()
        {
            ProxiedElement.Clear();
        }

        public void SendKeys(string text)
        {
            ProxiedElement.SendKeys(text);
        }

        public void Submit()
        {
            ProxiedElement.Submit();
        }

        public void Click()
        {
            ProxiedElement.Click();
        }

        public string GetAttribute(string attributeName)
        {
            return ProxiedElement.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return ProxiedElement.GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public string TagName => ProxiedElement.TagName;
        public string Text => ProxiedElement.Text;
        public bool Enabled => ProxiedElement.Enabled;
        public bool Selected => ProxiedElement.Selected;
        public Point Location => ProxiedElement.Location;
        public Size Size => ProxiedElement.Size;
        public bool Displayed => ProxiedElement.Displayed;

        public ProxiedWebElement FindElement(By by)
        {
            var element = ProxiedElement.FindElement(by);
            return new ProxiedWebElement(ScenarioContext, element);
        }

        public ReadOnlyCollection<ProxiedWebElement> FindElements(By by)
        {
            var elements = ProxiedElement.FindElements(by)
                .Select(e => new ProxiedWebElement(ScenarioContext, e))
                .ToArray();
            return new ReadOnlyCollection<ProxiedWebElement>(elements);
        }

        public bool IsImageVisible()
        {
            var script = ConfigManager.DriverType == "ie"
                ? "return arguments[0].complete"
                : "return (typeof arguments[0].naturalWidth!=\"undefined\"" +
                  " && arguments[0].naturalWidth>0)";
            var driver = ScenarioContext.Driver();
            return (bool) driver.ExecuteScript(script, ProxiedElement);
        }

        public void ScrollIntoView()
        {
            var driver = ScenarioContext.Driver();
            ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].scrollIntoView();", ProxiedElement);
        }

        public void ClickByWebElement()
        {
            var driver = ScenarioContext.Driver();
            ((IJavaScriptExecutor) driver).ExecuteScript("arguments[0].click();", ProxiedElement);
        }
    }
}