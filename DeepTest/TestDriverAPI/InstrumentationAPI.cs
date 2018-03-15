using System;
using System.Collections.Generic;

using Mono.Cecil;
using System.IO;

namespace TestDriverAPI
{
    public class InstrumentationAPI
    {
        protected Dictionary<string, AssemblyDefinition> mapAssemblyNamesToDefinitions;
        protected Dictionary<string, InstrumentationPoint> mapInstrumentationPointNamesToSpecifications;
        public MeasurementHandler Measure { get; }
        public SnapshotHandler Snapshot { get; }
        public DelayHandler Delay { get; }

        public InstrumentationAPI()
        {
            mapAssemblyNamesToDefinitions = new Dictionary<string, AssemblyDefinition>();
            mapInstrumentationPointNamesToSpecifications = new Dictionary<string, InstrumentationPoint>();
        
            Measure = new MeasurementHandler();
            Snapshot = new SnapshotHandler();
            Delay = new DelayHandler();
        }

        public void AddAssemblyFromPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath)) {
                throw new FileNotFoundException("AddAssemblyFromPath assembly not found: " + assemblyPath);
            }

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            Console.WriteLine("Successfully read assembly {0}", assembly.FullName);
            mapAssemblyNamesToDefinitions.Add(assembly.Name.Name, assembly);
        }

        public InstrumentationPoint AddNamedInstrumentationPoint(string name)
        {
            mapInstrumentationPointNamesToSpecifications.Add(name, new InstrumentationPoint(name, this));
            return mapInstrumentationPointNamesToSpecifications[name];
        }

        public AssemblyDefinition getAssemblyDefinitionByName(string assemblyFullName)
        {
            return mapAssemblyNamesToDefinitions[assemblyFullName];
        }
    
    
    
    }
}

