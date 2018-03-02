﻿using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using DeepTest;

namespace Test.NFBenchImport
{
    [TestFixture]
    public class TestBenchmarkPerformance
    {
        private DTRuntime dtr = new DTRuntime();

        // Staging Setup
        private static string stagingPath = 
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory))) + "/staging/";

        private static string nfbPerfPath = stagingPath + "NFBenchImport.Benchmark.Performance.exe";
        private static string clientPath = stagingPath + "NFBenchImport.Services.ClientApplication.exe";
        private static string wovenNfbPerfPath = stagingPath + "_woven_NFBenchImport.Benchmark.Performance.exe";

        [OneTimeSetUp]
        public void ReferenceFixtureSetUp()
        {
            DTNodeDefinition app = dtr.addSystemUnderTest(nfbPerfPath);

            WeavePoint onMessageReceive =
                dtr.Instrumentation.AddWeavePointOnEntry(
                    target: app,
                    nameOfWeavePointType: "PerformanceBugApplicationServer",
                    nameOfWeavePointMethod: "receiveMessageCallback"
                );
            WeavePoint onMessageResponseEnd =
                dtr.Instrumentation.AddWeavePointOnExit(
                    target: app,
                    nameOfWeavePointType: "PerformanceBugApplicationServer",
                    nameOfWeavePointMethod: "endMessageSendCallback"
                );

            dtr.Instrumentation.insertStopwatchAssertion(
                start: onMessageReceive,
                stop: onMessageResponseEnd
            );

            dtr.Instrumentation.Write(app, alternateWritePath: wovenNfbPerfPath);

            app.StartInstance(
                externalPath: wovenNfbPerfPath,
                argumentString: "reference 127.0.0.1 60728",
                workingDirectory: stagingPath
            );   
        }

        [Test]
        public void TestRoundtripMessage()
        {
            DTProcess client = new DTProcess(
                targetPath: clientPath,
                arguments: "127.0.0.1 60728 0",
                workingdir: stagingPath
            );
            client.Start();

            for (int i = 0; i < 3; i++) {
                client.SendMessageToComponentConsole("#0 Message " + i + " from DT.TestBenchmarkReference");
                Thread.Sleep(2000);
            }

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

