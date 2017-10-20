using System;
using FlowTest;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

namespace Test
{
    [TestFixture]
    public class ChatServerTests
    {
        private FlowTestRuntime rt = new FlowTestRuntime();

        private WeavePoint sendMessageWP;

        private static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
        private static string stagingTestDirectory = Directory.GetParent(workingTestDirectory).FullName + "/staging";
        private static string sourceExePath =  stagingTestDirectory + "/ChatServer.exe";
        private static string destinationExePath =  stagingTestDirectory + "/WovenChatServer.exe";
        private static string customCodeWeavePath = stagingTestDirectory + "/FlowTestInstrumentation.dll";
        private static string sourceClientPath = stagingTestDirectory + "/ChatClient.exe";

        [OneTimeSetUp]
        public void ChatSetup()
        {
            rt.Instrumentation.addExecutable(
                exeSourcePath: sourceExePath,
                exeWritePath: destinationExePath,
                argumentString: "7777",
                nSecondsDelay: 5
            );

            // Weaving 
            sendMessageWP = new WeavePoint(
                parentModule: sourceExePath,
                parentNamespace: "SampleServer",
                parentType: "ChatServer",
                methodToWatch: "SendMessage"
            );
            rt.Instrumentation.addWeavePoint(
                point: sendMessageWP,
                moduleWritePath: destinationExePath
            );
            rt.Instrumentation.write();
            rt.start();
           
        }

        [OneTimeTearDown]
        public void ChatTearDown()
        {
            rt.stopAndCleanup();
        }

        [TearDown]
        public void FormattingTearDown()
        {
            Console.WriteLine();
        }

        [Test]
        public void WeavingExampleTest()
        {
            /*ProcessExecutionWithRedirectedIO client1 = new ProcessExecutionWithRedirectedIO(
                targetPath: sourceClientPath,
                arguments: "7777"
            );
            ProcessExecutionWithRedirectedIO client2 = new ProcessExecutionWithRedirectedIO(
                targetPath: sourceClientPath,
                arguments: "7777"
            );
            client1.Start();
            client2.Start();

            client1.SendMessageToComponentConsole("Hi from Client 1");
            client2.SendMessageToComponentConsole("Hi from Client 2");

            List<FlowTestEvent> sendMsgEvents = rt.Events.eventsByWpKey(sendMessageWP.GetHashCode());
            Console.WriteLine(sendMsgEvents.Count);

            client1.Stop();
            client2.Stop();*/
        }
    }
}

