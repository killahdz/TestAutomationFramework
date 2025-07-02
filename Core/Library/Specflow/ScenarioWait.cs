using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenQA.Selenium;

namespace Core.Library.Specflow
{
    public class ScenarioWait
    {
        public ScenarioWait(IWebDriver driver, TimeSpan waitTime)
        {
            StepName = Hooks.CurrentSteps.Get(driver);
            WaitTimeInSeconds = waitTime.TotalSeconds;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenarioWaitId { get; set; }

        public double WaitTimeInSeconds { get; set; }

        public string StepName { get; set; }
    }
}