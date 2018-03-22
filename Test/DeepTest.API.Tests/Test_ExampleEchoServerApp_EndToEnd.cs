using System;
using System.IO;

using NUnit.Framework;

using DeepTestFramework;
using System.Threading;

namespace DeepTest.API.Tests
{
    [TestFixture]
    public class Test_ExampleEchoServerApp_EndToEnd
    {
        //[Test]
        public void ExampleEchoServer_InstrumentationDelay_ShouldIncreaseMessageRoundtrip()
        {
            InstrumentationAPI Instrumentation = new InstrumentationAPI();
            SystemUnderTestDeploymentAPI Driver = new SystemUnderTestDeploymentAPI();

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

            Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            InstrumentationPoint testDelayIp = 
                Instrumentation.AddNamedInstrumentationPoint("testDelayIP")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            Instrumentation.Delay
                .AddSecondsOfSleep(5)
                .StartingAtEntry(testDelayIp);
            
            using (SystemProcessWrapperWithInput sut = Driver.ExecuteWithArguments(instrumentedAppPath, "server 60013"))
            {
                TestUtility.mockUdpClientMessageRequest("127.0.0.1", 60013, "test");
            }
        }

        //[Test]
        public void ExampleEchoServer_MeasureStopwatch_ShouldCollectRoundtripMessageTimes()
        {
            InstrumentationAPI Instrumentation = new InstrumentationAPI();
            SystemUnderTestDeploymentAPI Driver = new SystemUnderTestDeploymentAPI();

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

            Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            InstrumentationPoint stopwatchStartPoint = 
                Instrumentation.AddNamedInstrumentationPoint("startStopwatchGotMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("GetAvailableMessage");
 
            InstrumentationPoint stopwatchEndPoint = 
                Instrumentation.AddNamedInstrumentationPoint("stopStopwatchSentMessage")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            Instrumentation.Measure
                .WithStopWatch()
                .StartingAtEntry(stopwatchStartPoint)
                .UntilExit(stopwatchEndPoint);
            
            using (SystemProcessWrapperWithInput sut = Driver.ExecuteWithArguments(instrumentedAppPath, "server 60012"))
            {
                TestUtility.mockUdpClientMessageRequest("127.0.0.1", 60012, "test");
            }
        }

        [Test]
        public void ExampleEchoServer_SnapshotValue_ShouldCollectInternalFields()
        {
            InstrumentationAPI Instrumentation = new InstrumentationAPI();
            SystemUnderTestDeploymentAPI Driver = new SystemUnderTestDeploymentAPI();

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

            Instrumentation.AddAssemblyFromPath(echoClientServerExamplePath);
            Instrumentation.SetAssemblyOutputPath(
                "ExampleClientServerEchoApp",
                instrumentedAppPath
            );

            // TODO --- make it easier to capture multiple snapshot values at one IP
            InstrumentationPoint snapshotPrivateInner = 
                Instrumentation.AddNamedInstrumentationPoint("snapshotClient")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");

            Instrumentation.Snapshot
                   .ValueOfField("theAnswer")
                   .StartingAtExit(snapshotPrivateInner);
            
            InstrumentationPoint snapshotNMessagesSent = 
                Instrumentation.AddNamedInstrumentationPoint("countMessagesSent")
                    .FindInAssemblyNamed("ExampleClientServerEchoApp")
                    .FindInTypeNamed("EchoServer")
                    .FindMethodNamed("RespondToMessage");
            
            Instrumentation.Snapshot
                   .ValueOfField("nMessagesSent")
                   .StartingAtExit(snapshotNMessagesSent);


            using (SystemProcessWrapperWithInput sut = Driver.ExecuteWithArguments(instrumentedAppPath, "server 60011"))
            {
                Assert.AreEqual(42, Driver.captureValue(snapshotPrivateInner));
                Assert.AreEqual(0, Driver.captureValue(snapshotNMessagesSent));

                TestUtility.mockUdpClientMessageRequest("127.0.0.1", 60011, "test 1");

                Assert.AreEqual(1, Driver.captureValue(snapshotNMessagesSent));
            }
        }
    }
}