using Core.Database;
using Core.Library;
using Core.Library.Extensions;
using Core.Library.Helpers;
using Core.Library.Media;
using Core.Models;
using Core.StepDefinitions;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using Core.Library.Specflow;
using Core.Library.WebDriver;
using OpenQA.Selenium.Interactions;
using TechTalk.SpecFlow;

namespace Core
{
    /// <summary>
    /// For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
    /// </summary>
    [Binding]
    public class Hooks : BaseStepDefinitions<BaseModel>
    {
        public static readonly ThreadSafeDictionary<IWebDriver, string> CurrentSteps =
            new ThreadSafeDictionary<IWebDriver, string>();

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        internal VideoCapture VideoRecorder;
        //private ConsoleLogSink _sink;

        public Hooks(ScenarioContext scenarioContext, FeatureContext featureContext) : base()
        {
            this._scenarioContext = scenarioContext ?? throw new ArgumentNullException("scenarioContext");
            this._featureContext = featureContext;
        }

        /// <summary>
        /// Web Driver Provider
        /// Use this to instantiate a new driver
        /// </summary>
        private WebDriverProvider _driverProvider = null;

        public WebDriverProvider DriverProvider => _driverProvider ?? (_driverProvider = new WebDriverProvider(_scenarioContext));

        private readonly string _scenarioConfigKey = "scenarioConfig";

        private ScenarioConfig ScenarioConfig
        {
            get => (ScenarioConfig) _scenarioContext[_scenarioConfigKey];
            set => _scenarioContext[_scenarioConfigKey] = value;
        }

        private readonly string _scenarioRunKey = "scenarioRun";

        private ScenarioRun ScenarioRun
        {
            get
            {
                if (!_scenarioContext.ContainsKey(_scenarioRunKey))
                    _scenarioContext[_scenarioRunKey] = new ScenarioRun(_featureContext,
                        _scenarioContext);
                return (ScenarioRun) _scenarioContext[_scenarioRunKey];
            }
        }

        private WebDriverProfile ScenarioWebDriverProfile => new WebDriverProfile(ScenarioConfig.IsMobileDisplayMode(), ConfigManager.WebDriverType);

        private bool Ignored { get; set; }

        #region Bindings

        /// <summary>
        /// Automation logic that has to run before the entire test run
        /// The method it is applied to must be static.
        /// </summary>
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Paths.CleanUpArtifacts();
        }

        /// <summary>
        /// Automation logic that has to run before executing each feature
        /// The method it is applied to must be static.
        /// </summary>
        [BeforeFeature]
        public static void BeforeFeature()
        {
        }

        /// <summary>
        /// Automation logic that has to run after executing each scenario or scenario outline example
        /// </summary>
        [BeforeScenario]
        public void BeforeScenario()
        {
            //check for ignore
            Ignored = !_scenarioContext.HasTag(ConfigManager.EnvTag);
            if (Ignored)
            {
                Assert.Ignore($"Test ignored for the target Environment: {ConfigManager.EnvTag}");
            }

            //check for console logging suppression
            //if (ParallelismHelper.IsInParallel())
            //    _sink = new ConsoleLogSink();

            //Store configuration
            ScenarioConfig = new ScenarioConfig(_featureContext.FeatureInfo,
                _scenarioContext.ScenarioInfo, Guid.NewGuid());

            //Fire up a web driver
            InitialiseDriver();

            if (ConfigManager.AcceptanceTestStatsLoggingEnabled)
                ScenarioRun.BeforeScenario();

            //Start Recording video
            if (ScenarioConfig.HasTag(Tags.RecordVideo) && ConfigManager.SupportsVideo)
            {
                VideoRecorder?.Dispose();
                VideoRecorder = new VideoCapture(Driver);
                VideoRecorder.Start(_scenarioContext.GetTestVariantName());
            }

            _failScreenshotTaken = false;
        }

        /// <summary>
        /// Automation logic that has to run after executing each scenario step
        /// </summary>
        [BeforeStep]
        public void BeforeStep()
        {
            CurrentSteps.Set(Driver.ProxiedDriver, StepContext.StepInfo.Text);
        }

        /// <summary>
        /// Automation logic that has to run after executing each scenario step
        /// </summary>
        [AfterStep]
        public void AfterStep()
        {
            if (_scenarioContext.TestError != null && !_failScreenshotTaken)
            {
                _scenarioContext.TakeScreenshot(_featureContext, "(Fail)");
                _failScreenshotTaken = true;
            }
        }

        private bool _failScreenshotTaken = false;

        /// <summary>
        /// Automation logic that has to run after executing each scenario or scenario outline example
        /// </summary>
        [AfterScenario]
        public void AfterScenario()
        {
            if (Ignored) return;

            //_sink?.Dispose();

            ScenarioRun.AfterScenario();
            //end video recording
            if (ScenarioConfig.HasTag(Tags.RecordVideo) && ConfigManager.SupportsVideo)
                VideoRecorder.End(Driver, _scenarioContext.TestError == null);

            //Log all the things
            if (ConfigManager.AcceptanceTestStatsLoggingEnabled)
                DatabaseContext.LogScenarioRun(ScenarioRun);

            //could log page source on error
            //clean up
            CurrentSteps.Remove(Driver.ProxiedDriver);
            ScenarioEventsRecorder.Remove(Driver.ProxiedDriver);

            //return the driver to the pool
            DriverProvider.Return(ScenarioWebDriverProfile, PooledDriver);
        }

        /// <summary>
        /// Automation logic that has to run after executing each feature
        /// The method it is applied to must be static.
        /// </summary>
        [AfterFeature]
        public static void AfterFeature()
        {
        }

        /// <summary>
        /// Note: As most of the unit test runners do not provide a hook for executing logic once the tests have been executed,
        /// the[AfterTestRun] event is triggered by the test assembly unload event.
        /// The exact timing and thread of this execution may therefore differ for each test runner.
        /// The method it is applied to must be static.
        /// </summary>
        [AfterTestRun]
        public static void AfterTestRun()
        {
            WebDriverProvider.CleanUp();
        }

        #endregion Bindings

        #region Helpers

        /// <summary>
        /// Gets a driver from the pool and registers it
        /// </summary>
        private void InitialiseDriver()
        {
            //check if the test has the reset tag
            var resetDriver = ScenarioConfig.HasTag(Tags.Reset);

            //Get a driver from the provider
            PooledDriver = DriverProvider.Borrow(ScenarioWebDriverProfile, resetDriver);
            //register the driver instance
            _scenarioContext.ScenarioContainer.RegisterInstanceAs(Driver, typeof(ProxiedWebDriver));

            //reset the driver
            if (resetDriver)
                PooledDriver.Value.ClearAndReset();
         
        }

        #endregion Helpers
    }
}