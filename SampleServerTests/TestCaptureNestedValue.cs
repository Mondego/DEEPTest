using System;
using FlowTestAPI;
using System.IO;
using System.Threading;
using SampleServer;
using NUnit.Framework;

namespace SampleServerTests
{
	[TestFixture]
	public class Test_CaptureNestedValueAtRuntime
	{
		FlowTestRuntime runtime;
		TargetComponentRuntime wovenComponent;

		string SourceComponentLocation = TestContext.CurrentContext.TestDirectory + "/SampleServer.exe";
		string WovenComponentLocation = TestContext.CurrentContext.TestDirectory + "/SampleServer.exe.woven";

		FlowTestPropertyOfInterest chatServerInnerValue;
		FlowTestPropertyOfInterest chatServerNMessagesSent;
		FlowTestPointOfInterest chatServerMsgSent;

		[OneTimeSetUp]
		public void SetUpFlowTest()
		{
			// Initialization 
			if (File.Exists(WovenComponentLocation))
			{
				Console.WriteLine ("Deleted woven component " + WovenComponentLocation);
				File.Delete(WovenComponentLocation);
			}

			Console.WriteLine ("=== One Time Setup ===");
			Console.WriteLine ("Source Component Path: " + SourceComponentLocation);
			Console.WriteLine ("Weaving Into Path: " + WovenComponentLocation);

			runtime = new FlowTestRuntime (
				sourceExecutable: SourceComponentLocation,
				destinationExecutable: WovenComponentLocation
			);

			chatServerMsgSent = new FlowTestPointOfInterest ("EchoServer.SendMessage");
			chatServerMsgSent.After();
			runtime.WatchPoint (chatServerMsgSent);

			chatServerInnerValue = new FlowTestPropertyOfInterest ("EchoServer.inner");
			runtime.WatchProperty (chatServerInnerValue);

			chatServerNMessagesSent = new FlowTestPropertyOfInterest("EchoServer.nMessagesSent");
			runtime.WatchProperty(chatServerNMessagesSent);

			runtime.Write ();

			// Execution
			string[] targetComponentArguments = new string[] { "7777" };
			wovenComponent = new TargetComponentRuntime (WovenComponentLocation, targetComponentArguments);

			Thread.Sleep (5000);
		}

		[OneTimeTearDown]
		public void TearDownFlowTest()
		{
			Console.WriteLine ("=== One Time Teardown ===");
			wovenComponent.Stop ();
			runtime.Stop ();
		}

		[Test]
		public void Test_CaptureNestedValue()
		{
			Thread.Sleep (5000);

			var poiRequestResult = chatServerInnerValue.GetPropertyFromRuntime (runtime);
			var expectedValue = 42;
		
			Assert.AreEqual(expectedValue, poiRequestResult);
		}

		[Test]
		public void Test_CountEchoServerMessagesSent()
		{
			string clientLocation = TestContext.CurrentContext.TestDirectory + "/SampleClient.exe";

			TargetComponentRuntime client1 = new TargetComponentRuntime(clientLocation, new string[] { "7777" });

			int expectedInitialNMessages = 0;
			var actualInitialNMessages = chatServerNMessagesSent.GetPropertyFromRuntime (runtime);
			Assert.AreEqual(expectedInitialNMessages, actualInitialNMessages);

			client1.SendMessageToComponentConsole("Hello from Client1");
			Thread.Sleep(1000);
			client1.Stop();

			int expectedNMessages = 1;
			var actualNMessages = chatServerNMessagesSent.GetPropertyFromRuntime (runtime);
			Assert.AreEqual(expectedNMessages, actualNMessages);
		}
			
		[TearDown]
		public void FormattingBetweenTests()
		{
			Console.WriteLine("====================");
		}
	}
}

