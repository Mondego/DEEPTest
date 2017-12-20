﻿using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using DeepTest;

namespace Test.Example.EchoChatDTSuite
{
    [TestFixture]
    public class TestEchoChatServerExample
    {
        private DTRuntime dtr = new DTRuntime();
        private string stagingDirectory = TestContext.CurrentContext.TestDirectory;

        [OneTimeSetUp]
        public void DeepTestFixtureSetUp()
        {
            Console.WriteLine(stagingDirectory);
            DTNodeDefinition echoServer = dtr.addSystemUnderTest(stagingDirectory + "/" + "Test.Example.EchoChatServer.exe");

            WeavePoint echoChatReceiveMsgWP = 
                dtr.Instrumentation.AddWeavePoint(echoServer, "EchoChatServer", "ReceiveMessageCallback");
            dtr.Instrumentation.Write(echoServer);

            echoServer.StartInstance(
                externalPath: stagingDirectory + "/Test.Example.EchoChatServer.exe",
                argumentString: "127.0.0.1 60708",
                nSecondsDelay: 0,
                workingDirectory: stagingDirectory
            );
        }
            
        [Test]
        public void TestPlaceholder()
        {
            Thread.Sleep(3000);

            UdpMessageSender helloEchoServer = new UdpMessageSender("127.0.0.1", 60708);
            helloEchoServer.SendMessage("Hello World!");

            Thread.Sleep(3000);
        }

        [TearDown]
        public void TearDownResultFormatting()
        {
            Console.WriteLine();
        }

        [OneTimeTearDown]
        public void DeepTestFixtureTearDown()
        {
            dtr.StopAll();
        }
    }
}

