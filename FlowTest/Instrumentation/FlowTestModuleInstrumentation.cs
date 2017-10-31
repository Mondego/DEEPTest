using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Mono.Cecil;
using System.Collections;
using System.Reflection;


namespace FlowTest
{
	public class FlowTestModuleInstrumentation
	{
        public Dictionary<string, AssemblyDefinition> mapOfReadPathsToAssemblyDefinitions { get; }
        public Dictionary<string, string> mapOfReadPathsToWritePaths { get; }
        public AssemblyDefinition flowTestInstrumentationHooks { get; }

		public FlowTestModuleInstrumentation ()
		{
            mapOfReadPathsToWritePaths = new Dictionary<string,string>();
            mapOfReadPathsToAssemblyDefinitions = new Dictionary<string, AssemblyDefinition>();
          
            string workingDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            string instrumentationHookPath = workingDirectory + "/FlowTestInstrumentation.dll";
            flowTestInstrumentationHooks = AssemblyDefinition.ReadAssembly(instrumentationHookPath);
		}
            
        /// <summary>
        /// Adds a weave point
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

                if (!mapOfReadPathsToAssemblyDefinitions.ContainsKey(point.moduleReadPath)) {
                    AssemblyDefinition targetModule = AssemblyDefinition.ReadAssembly(point.moduleReadPath);

                    mapOfReadPathsToAssemblyDefinitions.Add(
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
                }
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
                foreach (string moduleReadPath in mapOfReadPathsToAssemblyDefinitions.Keys) {
                    string moduleWritePath =  mapOfReadPathsToWritePaths[moduleReadPath];
                    AssemblyDefinition assemblyToWrite = mapOfReadPathsToAssemblyDefinitions[moduleReadPath];
                    assemblyToWrite.Write(moduleWritePath);
                }
            }

            catch (Exception e) {
                Console.WriteLine(
                    "FlowTestWeavingOrchestration.Write() caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }
	}
}

