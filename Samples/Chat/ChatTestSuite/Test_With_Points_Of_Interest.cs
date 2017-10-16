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
		WeavePoint pointOfMessageReceived, pointOfMessageSent;

		ProcessExecutionWithRedirectedIO client1;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string samplesParentDirectory = Directory.GetParent(workingTestDirectory).Parent.Parent.Parent.FullName;
		static string chatServerExecutablePath = samplesParentDirectory + "/Chat/SampleServer/bin/Debug/ChatServer.exe";
		static string chatClientExecutablePath = samplesParentDirectory + "/Chat/SampleClient/bin/Debug/ChatClient.exe";

		[OneTimeSetUp]
		public void ChatServerFlowTestSetup()
		{
			Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);

			// Initialize the runtime, which is the test driver for the flowtest
			runtime = new FlowTestRuntime();

			// Add the component to execute
            runtime.Instrumentation.addExecutable(
                exeSourcePath: chatServerExecutablePath,
                argumentString: "7777",
                nSecondsDelay: 5
            );

			// Points of interest where we want to weave some activity
			pointOfMessageSent = new WeavePoint (
				parentModule: chatServerExecutablePath,
				parentType: "ChatServer",
				methodToWatch: "SendMessage"
			);
            runtime.Instrumentation.addWeavePoint(pointOfMessageSent);

			pointOfMessageReceived = new WeavePoint (
				parentModule: chatServerExecutablePath,
				parentType: "ChatServer",
				methodToWatch: "ReceiveMessage"
			);
            runtime.Instrumentation.addWeavePoint(pointOfMessageReceived);

            runtime.Instrumentation.write();
			runtime.start();
		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.stopAndCleanup();
		}

		[Test]
		public void TestCase()
		{
			client1 = new ProcessExecutionWithRedirectedIO(
				targetPath: chatClientExecutablePath,
				arguments: "7777"
			);
			client1.Start();

			client1.SendMessageToComponentConsole("Client 1 - msg 1");

			Thread.Sleep(3000);
			client1.Stop();
		}
	}
}

