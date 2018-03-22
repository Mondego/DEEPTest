using System;
using System.IO;
using DeepTestFramework;
using NUnit.Framework;

namespace DeepTest.API.Tests
{
    [TestFixture]
    public class Test_InstrumentationAPI
    {
        [Test]
        public void LoadingAssemblyWithInvalidPath_ShouldFail()
        {
            InstrumentationAPI Instrumentation = new InstrumentationAPI();

            Assert.Throws<FileNotFoundException>(() => {
                Instrumentation.AddAssemblyFromPath(
                    TestContext.CurrentContext.TestDirectory + "non-existing.dll"
                );
            });
        }
    }
}
