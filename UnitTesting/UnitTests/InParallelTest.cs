using System;
using Core.Library.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting.UnitTests
{
    /// <summary>
    /// These tests check for the LevelOfParallelism attibute defined in the assemblySetup.cs file of the calling assembly
    /// These are adHoc tests. 
    /// One of them will fail if run as part of a test suite
    /// </summary>
    [TestClass]
    public class InParallelTest
    {
        [TestMethod]
        public void ShouldNotBeInParallel()
        {
            Assert.IsFalse(ParallelismHelper.IsInParallel());
        }

        [TestMethod]
        public void ShouldBeInParallel()
        {
            Assert.IsTrue(ParallelismHelper.IsInParallel());
        }
    }
}
