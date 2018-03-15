using System;
using System.IO;

using NUnit.Framework;

using TestDriverAPI;

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
            string echoClientServerExamplePath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/ExampleClientServerEchoApp.exe"
                );
            Console.WriteLine("Target system: {0}", echoClientServerExamplePath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);

            // TODO error handle so that that each of these throws if not found, or throws if too many
            InstrumentationPoint testDelayIp = 
                handler.Instrumentation.AddNamedInstrumentationPoint("testDelayIP")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");
            //testDelayIp.printMethodInstructions();

            handler.Instrumentation.Delay.AddSecondsOfSleep(5).AtEntry(testDelayIp);
        }

        [Test]
        public void ExampleEchoServer_MeasureStopwatch_ShouldCollectRoundtripMessageTimes()
        {
            string echoClientServerExamplePath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/ExampleClientServerEchoApp.exe"
                );
            Console.WriteLine("Target system: {0}", echoClientServerExamplePath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);

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

            handler.Instrumentation.Measure.WithStopWatch().AtEntry(stopwatchStartPoint).UntilExit(stopwatchEndPoint);
        }

        [Test]
        public void ExampleEchoServer_SnapshotValue_ShouldCollectInternalFields()
        {
            string echoClientServerExamplePath = 
                Path.Combine(
                    TestUtility.getRelativeSolutionPath(TestContext.CurrentContext.TestDirectory),
                    "staging/ExampleClientServerEchoApp.exe"
                );
            Console.WriteLine("Target system: {0}", echoClientServerExamplePath);

            DeepTestHandler handler = new DeepTestHandler();
            handler.Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);

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

            handler.Instrumentation.Snapshot.ValueOf("remote").AtExit(snapshotClient);
            handler.Instrumentation.Snapshot.ValueOf("nMessagesSent").AtExit(snapshotNMessagesSent);
        }
    }
}