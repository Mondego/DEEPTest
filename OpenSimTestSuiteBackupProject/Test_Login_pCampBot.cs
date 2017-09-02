using System;
using NUnit.Framework;
using FlowTest;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;

namespace OpenSimTestSuiteBackupProject
{
	[TestFixture]
	public class Test_Login_pCampBot
	{
		FlowTestRuntime runtime;
		ProcessWithIOHandler pCampBots, simulator1, robust;

		static string workingTestDirectory = TestContext.CurrentContext.TestDirectory;
		static string projectDirectory = Directory.GetParent(workingTestDirectory).Parent.FullName;
		static string temp = "/home/eugenia/Projects/opensim/bin/";

		[OneTimeSetUp]
		public void SetUpOpenSimTest()
		{
			runtime = new FlowTestRuntime();

			// Add the component to execute
			runtime.addAssemblyToFlowTest(
				pathToAssembly: temp + "Robust.exe",
				nSecondsRequiredAfterLaunch: 10,
				args: "",
				workingDirectory: temp
			);

			runtime.addAssemblyToFlowTest(
				pathToAssembly: temp + "OpenSim.exe",
				nSecondsRequiredAfterLaunch: 10,
				args: "",
				workingDirectory: temp
			);

			runtime.Write();
			runtime.Start();
		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.Stop();
		}

		[Test]
		public void TestLoginOnePCampBot()
		{
			pCampBots = new ProcessWithIOHandler(
				targetPath: temp + "pCampBot.exe",
				arguments: "-loginuri http://127.0.0.1:8002 -firstname Test -lastname Bot -password pw -c",
				workingdir: temp
			);
			pCampBots.Start();

			Thread.Sleep(60000);

			pCampBots.Stop();
		}
	}
}

