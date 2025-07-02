using Core.Library.Extensions;
using Core.Library.WebDriver;
using NUnit.Framework;

namespace UnitTesting.UnitTests
{
    /// <summary>
    /// Unit test fixture to test framework extension logic
    /// </summary>
    [TestFixture]
    public class ExtensionTest
    {
        [Test]
        public void CanGetChromeWebDriverProcessNameFromType()
        {
            var chromeType = WebDriverType.chrome;

            //verify driver process name
            var processAttr = chromeType.GetEnumAttribute<WebDriverProcessAttribute>();
            Assert.AreEqual("chromedriver", processAttr.Name);

            //verify browser process name
            var browserAttr = chromeType.GetEnumAttribute<BrowserProcessAttribute>();
            Assert.AreEqual("chrome", browserAttr.Name);
        }

        [Test]
        public void CanGetIeWebDriverProcessNameFromType()
        {
            var chromeType = WebDriverType.ie;

            //verify driver process name
            var processAttr = chromeType.GetEnumAttribute<WebDriverProcessAttribute>();
            Assert.AreEqual("iedriverserver", processAttr.Name);

            //verify browser process name
            var browserAttr = chromeType.GetEnumAttribute<BrowserProcessAttribute>();
            Assert.AreEqual("iexplore", browserAttr.Name);
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("hello", "hello")]
        [TestCase("HELLO", "hello")]
        [TestCase("hello", "HELLO")]
        [TestCase("HeLlo", "hELLO")]
        [TestCase(" HeLlo", "hELLO")]
        [TestCase("HeLlo", " hELLO ")]
        [TestCase(" HeLlo ", "            hELLO ")]
        public void SimilarStringMatchingShouldPass(string s1, string s2)
        {
            Assert.IsTrue(s1.IsSimilarTo(s2));
        }

        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase("", "hello")]
        [TestCase("HELLO1", "hello")]
        [TestCase("hello world", "hello")]
        [TestCase("hello", "hello  world")]
        [TestCase(" HeLlo ", "345fgvbcfc")]
        public void SimilarStringMatchingShouldFail(string s1, string s2)
        {
            Assert.IsFalse(s1.IsSimilarTo(s2));
        }
    }
}