
using System.IO;

using NUnit.Framework;

using FlowTest;

namespace Test
{
    [TestFixture]
    public class FlowTestHelloWorldTests
    {
        [Test]
        public void FlowTestAssemblyWriterCanExecuteFromMemory()
        {
            // Simple HelloWorld.exe file for testing
            string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
            string helloWorldExecutablePath = 
                Directory.GetParent(workingTestDirectory).Parent.Parent.FullName + "/Samples/HelloWorld/bin/Debug/HelloWorld.exe";
            Assert.True(
                File.Exists(helloWorldExecutablePath), 
                helloWorldExecutablePath + " not found");

            // FlowTestRunTime: execute HelloWorld without modifications
            FlowTestRuntime runtime = new FlowTestRuntime();
            runtime.addExecutableToFlowTestStartup(
                pathToExecutable: helloWorldExecutablePath,
                nSecondsOnLaunch: 0,
                arguments: "1"
            );
            runtime.Start();

            // Assert against runtime


            // Shutdown
            runtime.Stop();
        }
    }
}

