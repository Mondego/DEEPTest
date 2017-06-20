using System;
using NUnit.Framework;
using FlowTest;
using System.IO;

namespace ChatTestSuite
{
	[TestFixture]
	public class Test_With_Properties_Of_Interest
	{
		FlowTestRuntime runtime;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string samplesParentDirectory = Directory.GetParent(workingTestDirectory).Parent.Parent.Parent.FullName;
		static string chatServerExecutablePath = samplesParentDirectory + "/Chat/SampleServer/bin/Debug/ChatServer.exe";

		[OneTimeSetUp]
		public void FlowTestSetUp()
		{
			// Initialize the runtime, which is the test driver for the flowtest
			runtime = new FlowTestRuntime();


		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.Stop();
		}

		[Test]
		public void TestFetchPropertiesOfInterest()
		{
			Assert.True(true);
		}
	}
}

