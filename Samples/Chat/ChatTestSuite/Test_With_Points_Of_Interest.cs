using NUnit.Framework;
using System;
using FlowTest;
using System.IO;
using System.Threading;

namespace ChatTestSuite
{
	[TestFixture]
	public class Test_Points_Of_Interest_Weaving
	{
		FlowTestRuntime runtime;
		FlowTestPointOfInterest pointOfMessageReceived, pointOfMessageSent;

		ProcessWithIOHandler client1;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string samplesParentDirectory = Directory.GetParent(workingTestDirectory).Parent.Parent.Parent.FullName;
		static string chatServerExecutablePath = samplesParentDirectory + "/Chat/SampleServer/bin/Debug/ChatServer.exe";
		static string chatClientExecutablePath = samplesParentDirectory + "/Chat/SampleClient/bin/Debug/ChatClient.exe";

		[OneTimeSetUp]
		public void ChatServerFlowTestSetup()
		{
			// Initialize the runtime, which is the test driver for the flowtest
			runtime = new FlowTestRuntime();

			// Add the component to execute
			runtime.addAssemblyToExecuteInFlowTest(
				pathToAssembly: chatServerExecutablePath,
				nSecondsRequiredAfterLaunch: 5,
				args: new string[] { "7777" }
			);

			// Points of interest where we want to weave some activity
			pointOfMessageSent = new FlowTestPointOfInterest (
				parentModule: chatServerExecutablePath,
				parentObject: "ChatServer",
				methodToWatch: "SendMessage"
			);
			runtime.AddPointOfInterest(pointOfMessageSent);

			pointOfMessageReceived = new FlowTestPointOfInterest (
				parentModule: chatServerExecutablePath,
				parentObject: "ChatServer",
				methodToWatch: "ReceiveMessage"
			);
			runtime.AddPointOfInterest(pointOfMessageReceived);

			//runtime.Write();
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
			client1 = new ProcessWithIOHandler(
				targetPath: chatClientExecutablePath,
				targetArguments: new string[] { "7777" }
			);
			client1.Start();


			client1.SendMessageToComponentConsole("Client 1 - msg 1");


			Thread.Sleep(10000);
			client1.Stop();
		}
	}
}

