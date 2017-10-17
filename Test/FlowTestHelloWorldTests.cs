using System;
using System.IO;

using NUnit.Framework;

using FlowTest;


namespace Test
{
    [TestFixture]
    public class FlowTestHelloWorldExecutableTests
    {
        [Test]
        public void HelloWorldWithNoWeavePoints()
        {
            // Simple HelloWorld.exe file for testing
            string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
            string helloWorldExecutablePath = workingTestDirectory + "/HelloWorld.exe";

            Assert.True(
                File.Exists(helloWorldExecutablePath), 
                helloWorldExecutablePath + " not found");

            // FlowTestRunTime: execute HelloWorld without modifications
            FlowTestRuntime runtime = new FlowTestRuntime();
            runtime.Instrumentation.addExecutable(
                exeSourcePath: helloWorldExecutablePath,
                argumentString: "1"
            );
            runtime.start();

            Assert.AreEqual(
                expected: 1, 
                actual: runtime.getExecutableLog(helloWorldExecutablePath).Count, 
                message: "This execution should have yielded only one output");
            Assert.AreEqual(
                expected: "Hello World!", 
                actual: runtime.getExecutableLog(helloWorldExecutablePath)[0], 
                message: "The hello world example should have yielded a single statement: Hello World!");

            runtime.stopAndCleanup();
        }

        [Test]
        public void HelloWorldWithCustomWritePath()
        {
            // Simple HelloWorld.exe file for testing
            string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
            string stagingTestDirectory = Directory.GetParent(workingTestDirectory).FullName + "/staging";
    
            string sourceExePath =  stagingTestDirectory + "/HelloWorld.exe";
            string destinationExePath =  stagingTestDirectory + "/WovenHelloWorld.exe";
            string customCodeWeavePath = stagingTestDirectory + "/FlowTestInstrumentation.dll";

            Assert.True(File.Exists(sourceExePath), sourceExePath + " not found");

            // FlowTestRunTime: execute HelloWorld from a different write path
            FlowTestRuntime runtime = new FlowTestRuntime();
            runtime.Instrumentation.addExecutable(
                exeSourcePath: sourceExePath,
                exeWritePath: destinationExePath,
                argumentString: "1"
            );

            // Weaving 
            WeavePoint wp = new WeavePoint(
                parentModule: sourceExePath,
                parentNamespace: "HelloWorld",
                parentType: "SayHelloWorld",
                methodToWatch: "Hello"
            );
            runtime.Instrumentation.addWeavePoint(
                point: wp,
                moduleWritePath: destinationExePath
            );
            runtime.Instrumentation.write();

            Assert.True(File.Exists(destinationExePath), destinationExePath + " not found");
            runtime.start();

            // Shutdown
            runtime.stopAndCleanup();
            Assert.False(File.Exists(destinationExePath), destinationExePath + " should have been cleaned up");
        }
    }
}

