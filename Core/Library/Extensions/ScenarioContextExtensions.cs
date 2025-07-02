using System.Linq;
using Core.Library.Specflow;
using Core.Library.WebDriver;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Core.Library.Extensions
{
    public static class ScenarioContextExtensions
    {
        public static bool HasTag(this ScenarioContext context, string tag)
        {
            return context.ScenarioInfo.Tags.Any(t => t.Equals(tag));
        }

        public static ProxiedWebDriver Driver(this ScenarioContext scenarioContext)
        {
            var driver = scenarioContext.ScenarioContainer.Resolve<ProxiedWebDriver>();
            return driver;
        }

        public static ScenarioConfig ScenarioConfig(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<ScenarioConfig>("scenarioConfig");
        }

        /// <summary>
        /// If the test is a scenario outline this method returns
        /// [ScenarioOutlineName]_[TestRowName]
        /// else returns the scenario name
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public static string GetTestVariantName(this ScenarioContext si)
        {
            var title = si.ScenarioInfo.Title.Replace("_", "").Replace(".", "");
            var variantName = TestContext.CurrentContext.Test.Name.GetTestName().Replace("_", "").Replace(".", ""); 
            return title.Equals(variantName) 
                ? title 
                : $"{si.ScenarioInfo.Title}_{TestContext.CurrentContext.Test.Name.GetTestName()}";
        }
    }
}