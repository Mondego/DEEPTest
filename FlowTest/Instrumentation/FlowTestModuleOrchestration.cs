﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Mono.Cecil;
using System.Collections;


namespace FlowTest
{
	public class FlowTestModuleOrchestration
	{
        public Dictionary<string, ModuleDefinition> mapOfReadPathsToModuleDefinitions { get; }
        public OrderedDictionary mapOfExecutableSourcePathsToLaunchConfigurations { get; }
        public Dictionary<string, string> mapOfReadPathsToWritePaths { get; }

		public FlowTestModuleOrchestration ()
		{
			mapOfReadPathsToModuleDefinitions = new Dictionary<string, ModuleDefinition>();
            mapOfExecutableSourcePathsToLaunchConfigurations = new OrderedDictionary();
            mapOfReadPathsToWritePaths = new Dictionary<string,string>();
		}

        /// <summary>
        /// Add configuration data for an executable to run
        /// </summary>
        /// <param name="exeSourcePath">Path of executable from the system</param>
        /// <param name="exeWritePath">Destination where exe will be run</param>
        /// <param name="argumentString">string containing arguments, as they would be added on the command line</param>
        /// <param name="nSecondsDelay">Number of seconds needed for startup of executable</param>
        /// <param name="workingDirectory">Where this executable should be launched</param>
        public LaunchConfiguration addExecutable(
            string exeSourcePath,
            string exeWritePath = null,
            string argumentString = "",
            int nSecondsDelay = 0,
            string workingDirectory = null
        )
        {
            LaunchConfiguration config = 
                new LaunchConfiguration(
                    originalPath: exeSourcePath,
                    executionPath: exeWritePath,
                    arguments: argumentString,
                    nSecondsForStartup: nSecondsDelay,
                    workingDir: workingDirectory
                );

            mapOfExecutableSourcePathsToLaunchConfigurations.Add(
                key: exeSourcePath,
                value: config
            );

            return config;
        }

        /// <summary>
        /// Adds a weave point, initializing its module if needed. 
        /// </summary>
        /// <param name="point">A method to be instrumented in a given library or executable</param>
        /// <param name="moduleWritePath">Write path for when module is not being written in place</param>
		public void addWeavePoint(
			WeavePoint point,
            string moduleWritePath = null
		)
		{
			try
			{
				string poiModuleName = point.moduleReadPath; 

				if (!mapOfReadPathsToModuleDefinitions.ContainsKey(poiModuleName)) {
					ModuleDefinition targetModule = ModuleDefinition.ReadModule(poiModuleName);

					mapOfReadPathsToModuleDefinitions.Add(
						key: point.moduleReadPath,
						value: targetModule
					);

                    if (moduleWritePath != null)
                    {
                        mapOfReadPathsToWritePaths.Add(point.moduleReadPath, moduleWritePath);
                    }
                    else
                    {
                        mapOfReadPathsToWritePaths.Add(point.moduleReadPath, point.moduleReadPath);
                    }

                    // TODO 
					// BootstrapEventAggregation(targetModule);
				}

                // TODO
				//point.weaveIntoModule(mapOfReadPathsToModuleDefinitions[poiModuleName]);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.weavePointOfInterest(poi) caught unexpected [{0}] [{1}]",
					e.GetType(),
					e.Message);
			}
		}

        /// <summary>
        /// Write all woven modules to disk
        /// </summary>
        public void write()
        {
            try
            {
                foreach (string moduleReadPath in mapOfReadPathsToModuleDefinitions.Keys) {
                    string moduleWritePath =  mapOfReadPathsToWritePaths[moduleReadPath];
                    ModuleDefinition moduleToWrite = mapOfReadPathsToModuleDefinitions[moduleReadPath];
                    moduleToWrite.Write (moduleWritePath);
                }
            }

            catch (Exception e) {
                Console.WriteLine(
                    "FlowTestWeavingOrchestration.Write() caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }

        /// <summary>
        /// Gets std out data from a launch configuration
        /// </summary>
        /// <returns>The std out events from process.</returns>
        /// <param name="key">A string key, the original read path of the module</param>
        public List<string> getStdOutEventsFromLaunchConfig(string key)
        {
            return ((LaunchConfiguration)mapOfExecutableSourcePathsToLaunchConfigurations[key]).Log;
        }

        /// <summary>
        /// Start the ordered list of exes
        /// </summary>
        public void StartLaunchSequence()
        {
            foreach (DictionaryEntry exe in mapOfExecutableSourcePathsToLaunchConfigurations) {
                ((LaunchConfiguration)exe.Value).Start();
            }
        }

        /// <summary>
        /// Stops, in reverse order (naive) the exes
        /// </summary>
        public void StopLaunchSequence()
        {
            for (int i = mapOfExecutableSourcePathsToLaunchConfigurations.Count - 1; i >= 0; i--)
            {
                ((LaunchConfiguration)mapOfExecutableSourcePathsToLaunchConfigurations[i]).Stop();
            }
        }

        //////////////////////////

		private void BootstrapEventAggregation(
			ModuleDefinition m
		)
		{
			// Load Bootstrap
			WeavingFlowTestProxy.WeaveThreadSafeFlowTestProxyType (
				module: m,
				typeName: "FlowTestProxy"
			);
		}
	}
}

