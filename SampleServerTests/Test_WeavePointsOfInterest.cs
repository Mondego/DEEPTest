using System;
using NUnit.Framework;
using FlowTestAPI;
using System.IO;
using System.Threading;

namespace SampleServerTests
{
	[TestFixture]
	public class Test_WeavePointsOfInterest
	{
		FlowTestRuntime runtime;
		FlowTestPointOfInterest chatServerMsgSent;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string sourceComponentPath = workingTestDirectory + "/SampleServer.exe";

		[OneTimeSetUp]
		public void SetUpFlowTest()
		{
			runtime = new FlowTestRuntime(
				sourceExecutable: sourceComponentPath,
				destinationExecutable: sourceComponentPath + ".woven+points"
			);

			Console.WriteLine ("=== One Time Setup ===");
			Console.WriteLine ("Source Component Path: " + runtime.SourcePath);
			Console.WriteLine ("Weaving Into Component: " + runtime.InstrumentedPath);

			// Weaving a point of interest
			chatServerMsgSent = new FlowTestPointOfInterest ("EchoServer.SendMessage");
			chatServerMsgSent.After();
			runtime.WatchPoint (chatServerMsgSent);

			runtime.Write ();

			// Running a woven executable
			runtime.ExecuteWovenWithArguments("7777");
			Thread.Sleep(5000);
		}

		[OneTimeTearDown]
		public void TearDownFlowTest()
		{
			runtime.Stop ();
		}

		[Test]
		public void Test_CountEchoServerMessagesSent()
		{
			string chatClientLocation = workingTestDirectory + "/SampleClient.exe";
			string[] chatClientArguments = new string[] { "7777" };
			TargetComponentRuntime client1 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);
			TargetComponentRuntime client2 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);

			client1.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 1");

			client2.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 2");
			client2.SendMessageToComponentConsole("Client 2 - msg 1");

			Thread.Sleep(2000);
			client1.Stop();
			client2.Stop();
		}
	}
}

