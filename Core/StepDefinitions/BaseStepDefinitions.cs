using System;
using Core.Library.WebDriver;
using Core.Models;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.StepDefinitions
{
    [Binding]
    public class BaseStepDefinitions<TModel> : Steps where TModel : BaseModel
    {
        private TModel _model;

        public TModel Model => _model ?? (_model = (TModel) Activator.CreateInstance(typeof(TModel), ScenarioContext));

        public ProxiedWebDriver Driver => new ProxiedWebDriver(PooledDriver.Value, ScenarioContext);

        public PooledItem<IWebDriver> PooledDriver
        {
            get => ScenarioContext.Get<PooledItem<IWebDriver>>("PooledDriver");
            set => ScenarioContext.Set(value, "PooledDriver");
        }
    }
}