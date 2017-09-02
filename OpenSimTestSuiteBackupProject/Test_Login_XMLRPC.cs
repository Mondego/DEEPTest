using System;
using CookComputing.XmlRpc;
using NUnit.Framework;
using System.Collections;
using FlowTest;
using System.IO;

namespace OpenSimTestSuiteBackupProject
{
	[XmlRpcUrl("http://127.0.0.1:8002")] 
	public interface RemoteOpenSim : IXmlRpcProxy
	{ 
		[XmlRpcMethod("login_to_simulator")] 
		XmlRpcStruct login_to_simulator(XmlRpcStruct parameters);
	} 

	[TestFixture]
	public class Test_Login_XMLRPC
	{
		FlowTestRuntime runtime;
		ProcessWithIOHandler pCampBots, simulator1, robust;
		FlowTestPointOfInterest pointOfLoginServiceLogin;

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

			pointOfLoginServiceLogin = new FlowTestPointOfInterest (
				parentModule: temp + "OpenSim.Services.LLLoginService.dll",
				parentType: "LLLoginService",
				methodToWatch: "Login"
			);
			runtime.AddPointOfInterest(pointOfLoginServiceLogin);

			/*pointOfLoginServiceLogin .aLaCarteSystemCallBefore(
				systemType: typeof(System.Threading.Thread),
				methodName: "Sleep",
				parameters: new object[] { 121000 }
			);*/

			runtime.Write();
			runtime.Start();
		}

		[OneTimeTearDown]
		public void GenericTearDown()
		{
			runtime.Stop();
		}

		[Test]
		public void TestLoginXMLRPC()
		{
			XmlRpcStruct parameters = new XmlRpcStruct();
			XmlRpcStruct results = new XmlRpcStruct();

			RemoteOpenSim proxy = XmlRpcProxyGen.Create<RemoteOpenSim>();
			parameters.Add("first", "Test");
			parameters.Add("last", "Bot_0");
			parameters.Add("passwd", "pw");
			parameters.Add("start", "last");
			results = proxy.login_to_simulator(parameters);

			/*// 1. Login 
			// 2. Login again (should fail)zoom in on a problem - observable behavior wasn't obviously related. 
			Became apparent that an inventory download was very slow. "Zoom In" Hypothesis - "if I do this fix, this bug will go away"
			Artificial delay to repro. Reason of this bug was delay. One part of code using wrong api. 
			Nice flowtest - pCampBot - make sure non-functional requirement" pCampBots downloading invnetories
			when everything is downloaded shouldn't be more than x seconds. 

			*/
			// Login should be successful
			Assert.AreEqual("true", results["login"], results["message"].ToString());
			Console.WriteLine("xml test");
		}
	}
}

