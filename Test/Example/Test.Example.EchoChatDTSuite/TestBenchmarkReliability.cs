using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using DeepTest;

namespace Test.Example.EchoChatDTSuite
{
    [TestFixture]
    public class TestBenchmarkReliability
    {
        private DTRuntime dtr = new DTRuntime();
        private static string stagingDirectory = "/home/eugenia/Projects/_staging/";
        private static string location = "NFBench.Benchmark.Reliability.exe";
        private static string exePath = stagingDirectory + location;
        private static string clientPath = stagingDirectory + "TestClientApplications.exe";
        private static string wovenAppPath = stagingDirectory + "_woven_" + location;

        [OneTimeSetUp]
        public void ReliabilityFixtureSetUp()
        {
            DTNodeDefinition app = dtr.addSystemUnderTest(exePath);
            //WeavePoint echoChatReceiveMsgWP = 
            //   dtr.Instrumentation.AddWeavePoint(echoServer, "EchoChatServer", "ReceiveMessageCallback");
            dtr.Instrumentation.Write(app, alternateWritePath: wovenAppPath);

            app.StartInstance(
                externalPath: wovenAppPath,
                argumentString: "reliability 127.0.0.1 60718",
                workingDirectory: stagingDirectory
            );
        }
            
        //Test]
        public void TestRoundtripMessage()
        {
            DTProcess client = new DTProcess(
                targetPath: clientPath,
                arguments: "127.0.0.1 60718 0",
                workingdir: stagingDirectory
            );
            client.Start();

            client.SendMessageToComponentConsole("#0 Message from DT.TestBenchmarkReliability");
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

