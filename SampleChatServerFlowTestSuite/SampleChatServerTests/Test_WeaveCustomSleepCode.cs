using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using FlowTestAPI;

namespace SampleServerTests
{
	[TestFixture]
	public class Test_WeaveCustomSleepCode
	{
		FlowTestRuntime runtime;
		FlowTestPointOfInterest onChatServerReceiveMsg;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string sourceComponentPath = workingTestDirectory + "/SampleServer.exe";

		[OneTimeSetUp]
		public void SetUpFlowTest()
		{
			runtime = new FlowTestRuntime(
				sourceExecutable: sourceComponentPath,
				destinationExecutable: sourceComponentPath + ".woven+sleeps"
			);

			Console.WriteLine ("=== One Time Setup ===");
			Console.WriteLine ("Source Component Path: " + runtime.getSourceComponentPath(
			));
			Console.WriteLine ("Weaving Into Component: " + runtime.getDestinationComponentPath());

			// Weaving a point of interest
			/*onChatServerReceiveMsg = new FlowTestPointOfInterest (
				parentObject: "ChatServer", 
				methodToWatch: "ReceiveMessage",
				methodCallToWeave: typeof(UserDesignedWeaves.StaticMethodsCustomSleepWeave)
			);
			onChatServerReceiveMsg.watchAfter = false;
			runtime.WatchPoint (onChatServerReceiveMsg);*/
			runtime.Write ();

			// Running a woven executable
			runtime.ExecuteWovenWithArguments("7777");
			Thread.Sleep(5000);
		}

		[OneTimeTearDown]
		public void TearDownFlowTest()
		{
			Console.WriteLine ("Tearing down Custom Weaves FlowTest");
			runtime.Stop ();
		}

		[Test]
		public void Test_Sleep()
		{
			
			string chatClientLocation = workingTestDirectory + "/SampleClient.exe";
			string[] chatClientArguments = new string[] { "7777" };
			TargetComponentRuntime client1 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);

			client1.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 1");

			Thread.Sleep (3000);
			client1.Stop();
		}
	}
}

