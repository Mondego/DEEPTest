using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using DeepTestFramework;
using System.Threading.Tasks;
using InternalTestDriver;

namespace Test.NFBenchImport
{
    [TestFixture]
    public class TestBenchmarkReference
    {
        private DTRuntime dtr = new DTRuntime();

        // Staging Setup
        private static string stagingPath = 
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))) + "/staging/";

        private static string nfbRefPath = stagingPath + "NFBenchImport.Benchmark.Reference.exe";
        private static string clientPath = stagingPath + "NFBenchImport.Services.ClientApplication.exe";
        private static string wovenNfbRefPath = stagingPath + "_woven_NFBenchImport.Benchmark.Reference.exe";

        [OneTimeSetUp]
        public void ReferenceFixtureSetUp()
        {
            DTNodeDefinition app = dtr.addSystemUnderTest(nfbRefPath);

            WeavePoint onMessageReceive =
                dtr.weavingHandler.AddWeavePointOnEntry(
                    target: app,
                    nameOfWeavePointType: "ReferenceApplicationServer",
                    nameOfWeavePointMethod: "receiveMessageCallback"
                );
            WeavePoint onMessageResponseEnd =
                dtr.weavingHandler.AddWeavePointOnExit(
                    target: app,
                    nameOfWeavePointType: "ReferenceApplicationServer",
                    nameOfWeavePointMethod: "endMessageSendCallback"
                );

            Instrumentation.Stopwatch.From(wp1).To(Wp2).Collect();
            Instrumentation.AddSleepFor(n).At(wp1);
            Instrumentation.Count(wp1).Collect();
            Instrumentation.At(wp1).Collect("counter");

            dtr.weavingHandler.insertStopwatchAssertion(
                start: onMessageReceive,
                stop: onMessageResponseEnd
            );

            dtr.StartDriver();

            dtr.weavingHandler.Write(app, alternateWritePath: wovenNfbRefPath);

            app.StartInstance(
                externalPath: wovenNfbRefPath,
                argumentString: "reference 127.0.0.1 60708",
                workingDirectory: stagingPath
            );   
        }

        [Test]
        public void TestRoundtripMessage()
        {
            DTProcess client = new DTProcess(
                id: "client1",
                targetPath: clientPath,
                arguments: "127.0.0.1 60708 0",
                workingdir: stagingPath
            );
            client.Start();

            client.SendMessageToComponentConsole("#0 Message " + 0 + " from DT.TestBenchmarkReference");

            Assert.That(dtr.getResultByWeavePoint(wpEnd),Is.LessThanOrEqualTo(1000));
            dtr.getResults(wpEnd, "client1");
            dtr.getResults(wpEnd);
            dtr.getResults("client1");

            Thread.Sleep(1000);
            client.Stop();
        }

        [OneTimeTearDown]
        public void DeepTestFixtureTearDown()
        {
            dtr.StopAll();
        }
    }
}

