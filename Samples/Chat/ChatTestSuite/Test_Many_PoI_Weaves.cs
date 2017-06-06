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
		// TestAPI could call this a test driver
		FlowTestRuntime runtime;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string samplesParentDirectory = Directory.GetParent(workingTestDirectory).Parent.Parent.Parent.FullName;
		static string flowTestTargetComponentPath = samplesParentDirectory + "/Chat/SampleServer/bin/Debug/ChatServer.exe";

		// TODO - TestAPI functionality for cleaner name handling maybe?
		static string targetComponentLocation = Path.Combine(
			Directory.GetParent(workingTestDirectory).Parent.Parent.FullName, 
			"SampleServer/bin/Debug/ChatServer.exe");

		[OneTimeSetUp]
		public void FlowTestSetupManyPoints()
		{
			Console.WriteLine(samplesParentDirectory + "\n" + flowTestTargetComponentPath);
			runtime = new FlowTestRuntime(flowTestTargetComponentPath);



			runtime.ExecuteWovenWithArguments("7777");
			Thread.Sleep(5000);
		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.Stop();
		}

		[Test]
		public void TestCase()
		{
			string chatClientLocation = samplesParentDirectory + "/Chat/SampleClient/bin/Debug/ChatClient.exe";
			string[] chatClientArguments = new string[] { "7777" };
			TargetComponentRuntime client1 = new TargetComponentRuntime(chatClientLocation, chatClientArguments);

			client1.Start();
			client1.SendMessageToComponentConsole("Client 1 - msg 1");

			Thread.Sleep (3000);
			client1.Stop();
		}
	}
}

