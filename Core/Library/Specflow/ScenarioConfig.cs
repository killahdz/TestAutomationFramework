using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace Core.Library.Specflow
{
    public class ScenarioConfig
    {
        private readonly FeatureInfo _featureInfo;
        private readonly ScenarioInfo _scenarioInfo;

        public ScenarioConfig(FeatureInfo featureInfo, ScenarioInfo scenarioInfo, Guid testScenarioId)
        {
            TestScenarioId = testScenarioId;
            _featureInfo = featureInfo;
            _scenarioInfo = scenarioInfo;
        }

        public Guid TestScenarioId { get; }

        public bool IsMobileDisplayMode()
        {
            return _scenarioInfo.Tags.Any(s => s == "mobile")
                   || _scenarioInfo.Tags.All(s => s != "desktop");
        }

        public string FeatureTags()
        {
            return _featureInfo.Tags.Any() ? string.Join(" ", _featureInfo.Tags) : null;
        }

        public string ScenarioTags()
        {
            return _scenarioInfo.Tags.Any() ? string.Join(" ", _scenarioInfo.Tags) : null;
        }

        public bool HasTag(string tag)
        {
            return _featureInfo.Tags.Union(_scenarioInfo.Tags).Any(t => t == tag);
        }
    }
}