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

            // TODO error handle so that that each of these throws if not found, or throws if too many
            InstrumentationPoint testDelayIp = 
                handler.Instrumentation.AddNamedInstrumentationPoint("testDelayIP")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");
            //testDelayIp.printMethodInstructions();

            handler.Instrumentation.Delay
                .AddSecondsOfSleep(5)
                .AtEntry(testDelayIp);
            
            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60013");
            app.Start();
            app.StopAfterNSeconds(5);
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
            //stopwatchStartPoint.printMethodInstructions();
 
            InstrumentationPoint stopwatchEndPoint = 
                handler.Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");
            //stopwatchEndPoint.printMethodInstructions();

            handler.Instrumentation.Measure
                .WithStopWatch()
                .AtEntry(stopwatchStartPoint)
                .UntilExit(stopwatchEndPoint);
            
            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60012");
            app.Start();
            app.StopAfterNSeconds(5);
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
            //snapshotClient.printMethodInstructions();

            InstrumentationPoint snapshotNMessagesSent = 
                handler.Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");
            //snapshotNMessagesSent.printMethodInstructions();

            handler.Instrumentation.Snapshot
                .ValueOf("remote")
                .AtExit(snapshotClient);
            
            handler.Instrumentation.Snapshot
                .ValueOf("nMessagesSent")
                .AtExit(snapshotNMessagesSent);

            SystemProcessWithInput app = 
                handler.Deployment.ExecuteWithArguments(instrumentedAppPath, "server 60011");
            app.Start();
            app.StopAfterNSeconds(5);
        }
    }
}