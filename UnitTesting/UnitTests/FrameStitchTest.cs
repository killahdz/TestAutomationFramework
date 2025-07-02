using System;
using System.IO;
using Core.Library.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting.UnitTests
{
    [TestClass]
    public class FrameStitchTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var frameStitcher = new VideoCapture(null);
            frameStitcher.Stitch(@"c:/temp/stitch.wmv", @"c:/temp/frames");
        }
    }
}
