using System;
using System.IO;

using NUnit.Framework;

using DeepTestFramework;

namespace DeepTest.API.Tests
{
    [TestFixture]
    public class InstrumentationAPITests
    {
        [Test]
        public void LoadingAssemblyWithInvalidPath_ShouldFail()
        {
            DeepTestHandler handler = new DeepTestHandler();

            Assert.Throws<FileNotFoundException>(() => {
                handler.Instrumentation.AddAssemblyFromPath(
                    TestContext.CurrentContext.TestDirectory + "non-existing.dll"
                );
            });
        }

        [Test]
        public void ExampleEchoServer_InstrumentationDelay_ShouldIncreaseMessageRoundtrip()
        {
            string stagingPath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/"
                );
            string echoClientServerExamplePath = 
                Path.Combine(
                    stagingPath,
                    "ExampleClientServerEchoApp.exe"
                );
            string instrumentedAppPath = 
                Path.Combine(
                    stagingPath,
                    "Instrumented_DelayTest_ExampleClientServerEchoApp.exe"
                );

            Console.WriteLine("Instrumenting system: {0}", echoClientServerExamplePath);
            Console.WriteLine("Writing to: {0}", instrumentedAppPath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            handler.Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            InstrumentationPoint testDelayIp = 
                handler.Instrumentation.AddNamedInstrumentationPoint("testDelayIP")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            handler.Instrumentation.Delay
                .AddSecondsOfSleep(5)
                .StartingAtEntry(testDelayIp);
            
            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60013");
            app.Start();

            TestUtility.mockUdpClientRequest("127.0.0.1", 60013, "test");

            app.StopAfterNSeconds(20);
        }

        [Test]
        public void ExampleEchoServer_MeasureStopwatch_ShouldCollectRoundtripMessageTimes()
        {
            string stagingPath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/"
                );
            string echoClientServerExamplePath = 
                Path.Combine(
                    stagingPath,
                    "ExampleClientServerEchoApp.exe"
                );
            string instrumentedAppPath = 
                Path.Combine(
                    stagingPath,
                    "Instrumented_StopWatchTest_ExampleClientServerEchoApp.exe"
                );

            Console.WriteLine("Instrumenting system: {0}", echoClientServerExamplePath);
            Console.WriteLine("Writing to: {0}", instrumentedAppPath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            handler.Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            InstrumentationPoint stopwatchStartPoint = 
                handler.Instrumentation.AddNamedInstrumentationPoint("startStopwatchGotMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("GetAvailableMessage");
 
            InstrumentationPoint stopwatchEndPoint = 
                handler.Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            handler.Instrumentation.Measure
                .WithStopWatch()
                .StartingAtEntry(stopwatchStartPoint)
                .UntilExit(stopwatchEndPoint);
            
            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60012");
            app.Start();

            TestUtility.mockUdpClientRequest("127.0.0.1", 60012, "test");

            app.StopAfterNSeconds(20);
        }

        [Test]
        public void ExampleEchoServer_SnapshotValue_ShouldCollectInternalFields()
        {
            string stagingPath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/"
                );
            string echoClientServerExamplePath = 
                Path.Combine(
                    stagingPath,
                    "ExampleClientServerEchoApp.exe"
                );
            string instrumentedAppPath = 
                Path.Combine(
                    stagingPath,
                    "Instrumented_SnapshotTest_ExampleClientServerEchoApp.exe"
                );

            Console.WriteLine("Instrumenting system: {0}", echoClientServerExamplePath);
            Console.WriteLine("Writing to: {0}", instrumentedAppPath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            handler.Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            InstrumentationPoint snapshotClient = 
                handler.Instrumentation.AddNamedInstrumentationPoint("snapshotClient")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            InstrumentationPoint snapshotNMessagesSent = 
                handler.Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            handler.Instrumentation.Snapshot
                .ValueOfField("remote")
                .StartingAtExit(snapshotClient);
            
            handler.Instrumentation.Snapshot
                .ValueOfField("nMessagesSent")
                .StartingAtExit(snapshotNMessagesSent);

            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60011");
            app.Start();

            TestUtility.mockUdpClientRequest("127.0.0.1", 60011, "test");

            app.StopAfterNSeconds(20);
        }
    }
}