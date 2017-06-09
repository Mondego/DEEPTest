using NUnit.Framework;
using System;
using FlowTest;
using System.IO;
using System.Threading;

namespace ChatTestSuite
{
	[TestFixture]
	public class Test_Many_PoI_Weaves
	{
		FlowTestRuntime runtime;
		FlowTestPointOfInterest pointOfMessageReceived, pointOfMessageSent;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string samplesParentDirectory = Directory.GetParent(workingTestDirectory).Parent.Parent.Parent.FullName;
		static string chatServerExecutablePath = samplesParentDirectory + "/Chat/SampleServer/bin/Debug/ChatServer.exe";

		static string chatClientExecutablePath = samplesParentDirectory + "/Chat/SampleClient/bin/Debug/ChatClient.exe";

		[OneTimeSetUp]
		public void FlowTestSetupManyPoints()
		{
			// Initialize the runtime, which is the test driver for the flowtest
			runtime = new FlowTestRuntime();

			// Add the component to execute
			runtime.addAssemblyToFlowTest(
				pathToAssembly: chatServerExecutablePath,
				nSecondsRequiredAfterLaunch: 5,
				args: new string[] { "7777" }
			);

			// Points of interest where we want to weave some activity
			pointOfMessageReceived = new FlowTestPointOfInterest (
				parentModule: chatServerExecutablePath,
				parentObject: "ChatServer",
				methodToWatch: "ReceiveMessage"
			);
			runtime.AddWatchPoint(pointOfMessageReceived);

			pointOfMessageSent = new FlowTestPointOfInterest (
				parentModule: chatServerExecutablePath,
				parentObject: "ChatServer",
				methodToWatch: "SendMessage"
			);
			runtime.AddWatchPoint(pointOfMessageSent);

			runtime.Write();
			runtime.Start();
		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.Stop();
		}

		[Test]
		public void TestCase()
		{
			TargetComponentRuntime client1 = 
				new TargetComponentRuntime(
					targetPath: chatClientExecutablePath,
					targetArguments: new string[] { "7777" }
				);
			client1.Start();


			client1.SendMessageToComponentConsole("Client 1 - msg 1");


			Thread.Sleep(3000);
			client1.Stop();
		}
	}
}

