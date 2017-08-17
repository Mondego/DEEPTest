﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace FlowTest
{
	public class FlowTestWeavingOrchestration
	{
		private Dictionary<string, ModuleDefinition> mapOfReadPathsToModuleDefinitions;
		private Dictionary<string, string> mapOfReadPathsToWritePaths;

		public FlowTestWeavingOrchestration ()
		{
			mapOfReadPathsToModuleDefinitions = new Dictionary<string, ModuleDefinition>();
			mapOfReadPathsToWritePaths = new Dictionary<string, string>();
		}

		public void weavePointOfInterest(
			FlowTestPointOfInterest point
		)
		{
			try
			{
				string poiModuleName = point.parentModuleOfWatchpoint; 

				if (!mapOfReadPathsToModuleDefinitions.ContainsKey(poiModuleName)) {
					ModuleDefinition targetModule = ModuleDefinition.ReadModule(poiModuleName);

					mapOfReadPathsToModuleDefinitions.Add(
						key: point.parentModuleOfWatchpoint,
						value: targetModule
					);

					mapOfReadPathsToWritePaths[poiModuleName] = poiModuleName;

					BootstrapEventAggregation(targetModule);
				}

				Weaving.WeavePointofInterest (
					module: mapOfReadPathsToModuleDefinitions[poiModuleName],
					poi: point
				);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.weavePointOfInterest(poi) caught unexpected [{0}] [{1}]",
					e.GetType(),
					e.Message);
				Console.WriteLine("STACK: " + e.StackTrace);
				Console.WriteLine("INNER " + e.InnerException);
			}
		}

		private void BootstrapEventAggregation(
			ModuleDefinition m
		)
		{
			// Load Templates
			/*WeavingFlowTestProxy.WeaveTemplateLoader(
				module: m
			);*/

			// Load Bootstrap
			WeavingFlowTestProxy.WeaveThreadSafeFlowTestProxyType (
				module: m,
				typeName: "FlowTestProxy"
			);
		}

		public void Write(
		)
		{
			try
			{
				foreach (string moduleReadPath in mapOfReadPathsToModuleDefinitions.Keys) {
					string moduleWritePath = mapOfReadPathsToWritePaths[moduleReadPath];
					ModuleDefinition moduleToWrite = mapOfReadPathsToModuleDefinitions[moduleReadPath];
					moduleToWrite.Write (moduleWritePath);
				}
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.Write() caught unexpected {0} {1}",
					e.GetType(),
					e.Message);
			}
		}
	}
}

