using System;
using System.Linq;
using Mono.Cecil;

namespace FlowTest
{
	public class FlowTestWeaver
	{
		private ModuleDefinition mModule;
		private string flowTestWovenRuntimeHelperFieldName = "mWovenMessagesHandler"; 
		private string moduleReadPath;
		private string moduleWritePath;

		public FlowTestWeaver(string inPlaceWritePath)
		{
			moduleReadPath = inPlaceWritePath;
			moduleWritePath = inPlaceWritePath;
			mModule = ModuleDefinition.ReadModule(moduleReadPath);

		}

		public FlowTestWeaver(string sourceModulePath, string destinationModulePath)
		{
			moduleReadPath = sourceModulePath;
			moduleWritePath = destinationModulePath;
			mModule = ModuleDefinition.ReadModule(moduleReadPath);
		}

		private void weaveFlowTestAwayTeamHandler()
		{
			try {

				// TODO
				//mModule.Import(typeof(FlowTest));
				/*mModule.Import(typeof(FlowTestAwayTeam));
					
				// Weavethe custom field of type FlowTestAwayTeam to the module
				// entry point of the target component
				WeavingCustomFields.WeaveCustomFieldIntoClass(
					m: mModule,
					customFieldName: flowTestWovenRuntimeHelperFieldName,
					customFieldAttributes: Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
					customFieldType: typeof(FlowTestAwayTeam),
					destinationClassName: "MainClass"
				);

				// Initialize the newly woven field so we can use it to communicate with the FlowTestRuntime
				WeavingCustomFields.InitializeCustomField (
					destinationModule: mModule,
					destinationClassName: "MainClass",
					destinationMethodName: "Main",

					customFieldName: flowTestWovenRuntimeHelperFieldName,
					customFieldAttributes: Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
					customFieldType: typeof(FlowTestAwayTeam),
					customFieldConstructorArgTypes: new Type[] { typeof(int), typeof(int) },
					customFieldConstructorArgs: new object[] { 60011, 60012 }
				);*/
			}

			catch (Exception e)
			{
				Console.WriteLine("FlowTestWeaver.FlowTestWeaver caught exception e: " + e.Message);
			}
		}

		public void WriteInstrumentedCodeToFile()
		{
			try
			{
				mModule.Write (moduleWritePath);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeaver.WriteInstrumentedCodeToFile() caught unexpected exception " + e.GetType() + " " + e.Message);
			}

		}

		public void WeaveWatchpointAtPointOfInterest(FlowTestPointOfInterest point)
		{
			try
			{
				// TODO
				//WeavingAtLocation.WeaveModuleAtTargetPointCall (mModule, point);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeaver.WeaveWatchpointAtPointOfInterest(poi) caught unexpected " + e.GetType() + " " + e.Message);
			}
		}
	}
}
