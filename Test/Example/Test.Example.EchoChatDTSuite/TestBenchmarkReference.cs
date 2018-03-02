using System;
using NUnit.Framework;
using DeepTest;

namespace Test.Example.EchoChatDTSuite
{
    [TestFixture]
    public class TestBenchmarkReference
    {
        private DTRuntime dtr = new DTRuntime();
        private static string stagingDirectory = "/home/eugenia/Projects/NFBench-CSharp/NFBench.Tests/bin/Debug/";
        private static string location = "NFBench.Benchmark.ReferenceImplementation";
        private static string exePath = stagingDirectory + location;
        private static string clientPath = stagingDirectory + "TestClientApplications.exe";
        private static string wovenAppPath = stagingDirectory + "_woven_" + location;

        [OneTimeSetUp]
        public void ReferenceFixtureSetUp()
        {
            DTNodeDefinition app = dtr.addSystemUnderTest(exePath);


            dtr.Instrumentation.Write(app, alternateWritePath: wovenAppPath);

            app.StartInstance(
                externalPath: wovenAppPath,
                argumentString: "reference 127.0.0.1 60708",
                workingDirectory: stagingDirectory
            );
        }

        [OneTimeTearDown]
        public void DeepTestFixtureTearDown()
        {
            dtr.StopAll();
        }
    }
}

