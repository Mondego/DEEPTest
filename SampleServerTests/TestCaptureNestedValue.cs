using System;
using FlowTestAPI;
using NUnit.Framework;
using System.IO;
using System.Threading;
using SampleServer;

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

			/*chatServerMsgSent = new FlowTestPointOfInterest ("EchoServer.SendMessage");
			chatServerMsgSent.After();
			chatServerMsgSent.Before();
			runtime.WatchPoint (chatServerMsgSent);*/

			chatServerInnerValue = new FlowTestPropertyOfInterest ("EchoServer.inner");
			runtime.WatchProperty (chatServerInnerValue);
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
			
		[TearDown]
		public void FormattingBetweenTests()
		{
			Console.WriteLine("====================");
		}
	}
}

