using System;
using System.Collections.Generic;
using Core.Library.WebDriver;
using OpenQA.Selenium;

namespace Core.Library.Specflow
{
    public static class ScenarioEventsRecorder
    {
        public static ThreadSafeDictionary<IWebDriver, List<ScenarioWait>> ScenarioWaits
            = new ThreadSafeDictionary<IWebDriver, List<ScenarioWait>>();

        public static ThreadSafeDictionary<IWebDriver, List<ScenarioLog>> ScenarioLogs
            = new ThreadSafeDictionary<IWebDriver, List<ScenarioLog>>();

        public static void Initialise(IWebDriver driver)
        {
            ScenarioWaits.Set(driver, new List<ScenarioWait>());
            ScenarioLogs.Set(driver, new List<ScenarioLog>());
        }

        public static void Remove(IWebDriver driver)
        {
            ScenarioWaits.Remove(driver);
            ScenarioLogs.Remove(driver);
        }

        public static void RecordWait(this IWebDriver driver, TimeSpan elapsed)
        {
            ScenarioWaits
                .GetOrAdd(driver, (d) => new List<ScenarioWait>())
                .Add(new ScenarioWait(driver, elapsed));
        }

        public static void RecordLog(this IWebDriver driver, ScenarioLog log)
        {
            ScenarioLogs
                .GetOrAdd(driver, (d) => new List<ScenarioLog>())
                .Add(log);
        }

        public static void RecordLog(this ProxiedWebDriver driver, string message)
        {
            ScenarioLogs
                .GetOrAdd(driver.ProxiedDriver, (d) => new List<ScenarioLog>())
                .Add(new ScenarioLog {Message = message});
        }
    }
}