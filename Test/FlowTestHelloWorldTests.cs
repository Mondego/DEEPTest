using System;
using System.IO;

using NUnit.Framework;

using FlowTest;
using System.Threading;
using System.Collections.Generic;


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
            string helloWorldExecutablePath = Directory.GetParent(workingTestDirectory).FullName + "/staging/HelloWorld.exe";

            // FlowTestRunTime: execute HelloWorld without modifications
            FlowTestRuntime runtime = new FlowTestRuntime();
 
            FTProcess hw = runtime.Execute(
                executablePath: helloWorldExecutablePath,
                argumentString: "1"
            );

            hw.Stop();
            runtime.Stop();
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

            // FlowTestRunTime: execute HelloWorld from a different write path
            FlowTestRuntime runtime = new FlowTestRuntime();

            // Weaving 
            WeavePoint wp = new WeavePoint(
                parentModule: sourceExePath,
                parentNamespace: "HelloWorld",
                parentType: "SayHelloWorld",
                methodToWatch: "Hello"
            );
            runtime.instrumentation.addWeavePoint(
                point: wp,
                moduleWritePath: destinationExePath
            );
            runtime.instrumentation.write();

            // Run anything needed for the tests
            FTProcess hw = runtime.Execute(
                executablePath: destinationExePath,
                argumentString: "1"
            );

            // Shutdown
            hw.Stop();
            runtime.Stop();
        }

        [TearDown]
        public void Formatting()
        {
            Console.WriteLine();
        }
    }
}

