using System;
using NUnit.Framework;
using DeepTest;
using System.Threading;

namespace Test.Example.EchoChatDTSuite
{
    [TestFixture]
    public class TestBenchmarkSecurity
    {
        private DTRuntime dtr = new DTRuntime();
        private static string stagingDirectory = "/home/eugenia/Projects/_staging/";
        private static string location = "NFBench.Benchmark.Security.exe";
        private static string exePath = stagingDirectory + location;
        private static string clientPath = stagingDirectory + "TestClientApplications.exe";
        private static string wovenAppPath = stagingDirectory + "_woven_" + location;

        [OneTimeSetUp]
        public void SecurityFixtureSetUp()
        {
            DTNodeDefinition app = dtr.addSystemUnderTest(exePath);


            dtr.Instrumentation.Write(app, alternateWritePath: wovenAppPath);

            app.StartInstance(
                externalPath: wovenAppPath,
                argumentString: "security 127.0.0.1 60738",
                workingDirectory: stagingDirectory
            );
        }

        //[Test]
        public void TestRoundtripMessage()
        {
            DTProcess client = new DTProcess(
                targetPath: clientPath,
                arguments: "127.0.0.1 60738 0",
                workingdir: stagingDirectory
            );
            client.Start();

            client.SendMessageToComponentConsole("#0 Message from DT.TestBenchmarkSecurity");
            Thread.Sleep(5000);

            client.Stop();
        }

        [OneTimeTearDown]
        public void DeepTestFixtureTearDown()
        {
            dtr.StopAll();
        }
    }
}

