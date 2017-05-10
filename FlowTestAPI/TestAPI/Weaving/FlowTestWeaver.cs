﻿using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FlowTestAPI
{
	public class FlowTestWeaver
	{
		private ModuleDefinition mModule;
		private string flowTestWovenRuntimeHelperFieldName = "mWovenMessagesHandler"; 
		private string moduleReadPath;
		private string moduleWritePath;

		public FlowTestWeaver(string sourceModulePath, string destinationModulePath)
		{
			moduleReadPath = sourceModulePath;
			moduleWritePath = destinationModulePath;
			mModule = ModuleDefinition.ReadModule(moduleReadPath);

			weaveFlowTestAwayTeamHandler ();
		}

		private void weaveFlowTestAwayTeamHandler()
		{
			try {
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
				);

				// NOTE: Previously, an entire object was registered with the FlowTestAwayTeam and had to be 
				// searched. Now, only specific fields are searched for; see FlowTestPropertyOfInterest for a demo
				// of this sort of weaving.
			}

			catch (Exception e)
			{
				Console.WriteLine("FlowTestWeaver caught exception e: " + e.Message);
			}
		}

		public void WriteInstrumentedCodeToFile()
		{
			mModule.Write (moduleWritePath);
		}

		// Points of Interest
		public void WeaveWatchpointAtPointOfInterest(FlowTestPointOfInterest point)
		{
			WeavingAtLocation.WeaveModuleAtTargetPointCall (mModule, point);
		}
	}
}
