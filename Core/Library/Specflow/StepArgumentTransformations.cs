using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Core.Library.Specflow
{
    [Binding]
    public class StepArgumentTransformations
    {
        [StepArgumentTransformation(@"((?:un)?check(?:ed)?)")]
        public bool CheckedUncheckedTransform(string value)
        {
            var possibleValues = new Dictionary<string, bool>
            {
                {"checked", true},
                {"check", true},
                {"unchecked", false},
                {"uncheck", false}
            };

            return possibleValues[value.ToLower()];
        }
    }
}