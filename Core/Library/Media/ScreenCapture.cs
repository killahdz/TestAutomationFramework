using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Core.Library.Extensions;
using Core.Library.Specflow;
using Core.Library.WebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Library.Media
{
    public static class ScreenCapture
    {
        public static string TakeScreenshot(this ScenarioContext scenarioContext, FeatureContext featureContext,
            string prefix)
        {
            if (!WebDriverProvider.Settings.TakeScreenShots)
                return null;
            try
            {
                //get feature title for sub dir
                var featureTitle = featureContext.FeatureInfo.Title;
                var testName = TestContext.CurrentContext.Test.Name.GetTestName();
                var stepTitle = scenarioContext.StepContext.StepInfo.Text;
                stepTitle = stepTitle.Substring(0, new[] { 30, stepTitle.Length }.Min());

                var filename = $"{DateTime.Now:yyyyMMddhhmmssff}_{prefix}_{stepTitle}";

                //build outputPath
                var outputPath = Paths.Media.GetImagePath(featureTitle, testName, filename);

                //take the screenshot
                scenarioContext.Driver().SaveScreenshot(outputPath);

                //verify
                if (!File.Exists(outputPath))
                    throw new FileNotFoundException($"Screenshot does not exist at {outputPath}");

                Console.WriteLine($" =======> {prefix} Screenshot saved to {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                scenarioContext.Driver().RecordLog($"Failed to take screenshot. {ex}");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="path"></param>
        public static void SaveScreenshot(this ProxiedWebDriver driver, string path)
        {
            if (!WebDriverProvider.Settings.TakeScreenShots)
                return;

            Screenshot screenshot = null;

            var _driver = driver.ProxiedDriver;

            // Take screenshot with webdriver
            var screenshotDriver = _driver as ITakesScreenshot;
            if (screenshotDriver == null)
                throw new NullReferenceException($"screenshotDriver. ProxiedDriver null = {_driver == null}");

            //get the screenshot in memory
            screenshot = screenshotDriver.GetScreenshot();
            var outputPath = path;

            try
            {
                //clone the image so we are not contending for memory
                using (var ms = new MemoryStream(screenshot.AsByteArray))
                {
                    using (var screenShotImage = Image.FromStream(ms))
                    {
                        var cp = new Bitmap(screenShotImage);
                        //write to disk
                        cp.Save(outputPath, ImageFormat.Jpeg);
                        cp.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                driver.RecordLog($"Save Frame failed. Ouput path: '{outputPath}' exists: '{Directory.Exists(outputPath)}'" + e.ToString());
                Console.WriteLine(e);
            }



        }
    }
}