﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;

namespace DeepTestFramework
{
    public class InstrumentationAPI
    {
        private Dictionary<string, AssemblyDefinition> mapAssemblyNamesToDefinitions;
        private Dictionary<string, InstrumentationPoint> mapInstrumentationPointNamesToSpecifications;
        private Dictionary<string, string> mapAssemblyNamesToWritePaths;

        public MeasurementHandler Measure { get; }
        public SnapshotHandler Snapshot { get; }
        public DelayHandler Delay { get; }

        public InstrumentationAPI()
        {
            mapAssemblyNamesToDefinitions = new Dictionary<string, AssemblyDefinition>();
            mapAssemblyNamesToWritePaths = new Dictionary<string, string>();
            mapInstrumentationPointNamesToSpecifications = new Dictionary<string, InstrumentationPoint>();

            Measure = new MeasurementHandler(this);
            Snapshot = new SnapshotHandler(this);
            Delay = new DelayHandler(this);
        }

        public InstrumentationAPI AddAssemblyFromPath(string assemblyPath)
        {
            if (!File.Exists(assemblyPath)) {
                throw new FileNotFoundException("AddAssemblyFromPath assembly not found: " + assemblyPath);
            }

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            Console.WriteLine("Successfully read assembly {0}", assembly.FullName);
            mapAssemblyNamesToDefinitions.Add(assembly.Name.Name, assembly);

            return this;
        }

        public void EnableBootstrap()
        {
            BootstrapBrokerInstanceHelper bootstrapHelper = new BootstrapBrokerInstanceHelper(this);
            InstrumentationAPI iapi = this;

            foreach (InstrumentationPoint i in mapInstrumentationPointNamesToSpecifications.Values) {
                InstrumentationPoint ctor = 
                    new InstrumentationPoint("ctor", iapi)
                        .FindInAssemblyNamed(i.instrumentationPointAssemblyDefinition.Name.Name)
                        .FindInTypeNamed(i.instrumentationPointTypeDefinition.Name)
                        .FindMethodNamed(".ctor");
                bootstrapHelper.StartingAtEntry(ctor);
            }
        }

        public void updateAssembly(InstrumentationPoint ip)
        {
            mapAssemblyNamesToDefinitions[ip.AssemblyName] = ip.instrumentationPointAssemblyDefinition;

            // Add bootstrapper code to get around the async deadlock bug
            /*
            InstrumentationPoint ctor = 
                new InstrumentationPoint("ctor", this)
                    .FindInAssemblyNamed(ip.instrumentationPointAssemblyDefinition.Name.Name)
                    .FindInTypeNamed(ip.instrumentationPointTypeDefinition.Name)
                    .FindMethodNamed(".ctor");
            bootstrapHelper.StartingAtEntry(ctor);*/
        }

        public void writeAssembly(InstrumentationPoint ip)
        {
            AssemblyDefinition assemblyToWrite = mapAssemblyNamesToDefinitions[ip.AssemblyName];
            string writePath = mapAssemblyNamesToWritePaths[ip.AssemblyName];
            assemblyToWrite.Write(writePath);
        }

        public void SetAssemblyOutputPath(string source, string writePath)
        {
            mapAssemblyNamesToWritePaths.Add(source, writePath);
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

