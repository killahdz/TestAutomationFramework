using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Library.WebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Library.Specflow
{
    public class ScenarioRun
    {
        private const string PreviousScenarioTitleKey = "previousScenarioTitle";
        private const string ScenarioVariantKey = "scenarioVariant";

        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;

        #region Ctor

        public ScenarioRun(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

        #endregion Ctor

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenarioRunId { get; set; }

        public int ScenarioId { get; set; }
        public int ScenarioVariant { get; set; }
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public List<ScenarioWait> ScenarioWaits => GetScenarioWaits();
        public List<ScenarioLog> ScenarioLogs => GetScenarioLogs();

        public string Host
        {
            get { return Environment.MachineName; }
            set { }
        }

        public string Thread
        {
            get { return System.Threading.Thread.CurrentThread.Name; }
            set { }
        }

        public string FeatureName
        {
            get { return _featureContext.FeatureInfo.Title; }
            set { }
        }

        public string ScenarioName
        {
            get { return _scenarioContext.ScenarioInfo.Title; }
            set { }
        }

        public string TestName
        {
            get { return TestContext.CurrentContext.Test.Name; }
            set { }
        }

        public string WebRoot
        {
            get { return ConfigManager.WebsiteRoot; }
            set { }
        }

        public string CountryCode
        {
            get { return "AU"; }
            set { }
        }

        public string Culture
        {
            get { return "EN"; }
            set { }
        }

        public string FeatureTags
        {
            get { return ScenarioConfig().FeatureTags(); }
            set { }
        }

        public string ScenarioTags
        {
            get { return ScenarioConfig().ScenarioTags(); }
            set { }
        }

        public double DurationInSeconds
        {
            get { return EndDateTime.Subtract(StartDateTime).TotalSeconds; }
            set { }
        }

        public bool Passed
        {
            get { return _scenarioContext.TestError == null; }
            set { }
        }

        public string Exception
        {
            get { return _scenarioContext != null && _scenarioContext.TestError == null ? null : _scenarioContext.TestError.ToString(); }
            set { }
        }

        #region Hooks

        /// <summary>
        ///     Called from Hooks
        /// </summary>
        public void BeforeScenario()
        {
            StartDateTime = DateTime.Now;

            ScenarioVariant = 0;
            ScenarioEventsRecorder.Initialise(Driver());

            if (_featureContext.ContainsKey(PreviousScenarioTitleKey))
            {
                var previousScenarioTitle = _featureContext.Get<string>(PreviousScenarioTitleKey);
                if (_scenarioContext.ScenarioInfo.Title == previousScenarioTitle)
                {
                    ScenarioVariant = _featureContext.Get<int>(ScenarioVariantKey);
                    ScenarioVariant++;
                }
            }

            _featureContext.Set(ScenarioVariant, ScenarioVariantKey);
        }

        /// <summary>
        ///     Called from Hooks
        /// </summary>
        public void AfterScenario()
        {
            EndDateTime = DateTime.Now;
            _featureContext.Set(_scenarioContext.ScenarioInfo.Title, PreviousScenarioTitleKey);
        }

        #endregion Hooks

        #region Helper accessors

        private IWebDriver Driver()
        {
            return _scenarioContext.Get<PooledItem<IWebDriver>>("PooledDriver").Value;
        }

        private ScenarioConfig ScenarioConfig()
        {
            return (ScenarioConfig) _scenarioContext["scenarioConfig"];
        }

        public List<ScenarioWait> GetScenarioWaits()
        {
            return ScenarioEventsRecorder.ScenarioWaits.Get(Driver());
        }

        public List<ScenarioLog> GetScenarioLogs()
        {
            return ScenarioEventsRecorder.ScenarioLogs.Get(Driver());
        }

        #endregion Helper accessors
    }
}