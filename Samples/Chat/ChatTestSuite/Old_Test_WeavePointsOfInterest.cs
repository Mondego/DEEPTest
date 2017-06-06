using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using FlowTestAPI;

namespace SampleServerTests
{
	[TestFixture]
	public class Test_WeavePointsOfInterest
	{
		FlowTestRuntime runtime;
		FlowTestPointOfInterest chatServerMsgSent;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string sourceComponentPath = workingTestDirectory + "/SampleServer.exe";

		//[OneTimeSetUp]
		public void SetUpFlowTest()
		{
			runtime = new FlowTestRuntime(
				sourceExecutable: sourceComponentPath,
				destinationExecutable: sourceComponentPath + ".woven+points"
			);

			Console.WriteLine ("=== One Time Setup ===");
			Console.WriteLine ("Source Component Path: " + runtime.getSourceComponentPath(
			));
			Console.WriteLine ("Weaving Into Component: " + runtime.getDestinationComponentPath());

			// Weaving a point of interest
			chatServerMsgSent = new FlowTestPointOfInterest (
				parentObject: "ChatServer", 
				methodToWatch: "SendMessage"
			);
			chatServerMsgSent.watchBefore = false;
			runtime.WatchPoint (chatServerMsgSent);
			runtime.Write ();

			// Running a woven executable
			runtime.ExecuteWovenWithArguments("7777");
			Thread.Sleep(5000);
		}

		//[OneTimeTearDown]
		public void TearDownFlowTest()
		{
			Console.WriteLine ("Tearing down Points of Interest FlowTest");
			runtime.Stop ();
		}

		//[Test]
		public void Test_CountEchoServerMessagesSent()
		{
			string chatClientLocation = workingTestDirectory + "/SampleClient.exe";
			string[] chatClientArguments = new string[] { "7777" };
			TargetComponentRuntime client1 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);
			TargetComponentRuntime client2 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);

			client1.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 1");
			Thread.Sleep (3000);
			FlowTestInstrumentationEvent[] res1 = chatServerMsgSent.getTestResults();
			Assert.AreEqual(1, res1.Length, "One message should have been sent");

			client2.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 2");
			Thread.Sleep (3000);
			FlowTestInstrumentationEvent[] res2 = chatServerMsgSent.getTestResults ();
			Assert.AreEqual(2, res2.Length, "Two messages should have been sent - server doesn't know client 2 yet");

			client2.SendMessageToComponentConsole("Client 2 - msg 1");
			Thread.Sleep(3000);
			FlowTestInstrumentationEvent[] res3 = chatServerMsgSent.getTestResults ();
			Assert.AreEqual(4, res3.Length, "Four messages should have been sent, with 2 clients accounted for.");

			client1.Stop();
			client2.Stop();
		}
	}
}

