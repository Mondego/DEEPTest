using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

using RemoteTestingWrapper;

namespace DeepTestFramework
{
    public class LegacyWeavingHandler
    {
        private AssemblyDefinition mPlugin;
        private Dictionary<string, AssemblyDefinition> mWeaves = 
            new Dictionary<string, AssemblyDefinition>();
        private int tempMetadata;

        public LegacyWeavingHandler(int metadata)
        {
            System.Reflection.AssemblyName pluginAssemblyMetadata =
                System.Reflection.Assembly.GetExecutingAssembly()
                    .GetReferencedAssemblies()
                    .Single(an => an.Name == "RemoteTestingWrapper");
           
            mPlugin = AssemblyDefinition.ReadAssembly(
                System.Reflection.Assembly.ReflectionOnlyLoad(
                    pluginAssemblyMetadata.FullName).Location);

            tempMetadata = metadata;
        }

        public void insertStopwatchAssertion(InstrumentationPoint start, InstrumentationPoint stop)
        {
            // TODO move this to RemoteAssertionHelper
            /*ILProcessor ilp = stop.instrumentationPointMethodDefinition.Body.GetILProcessor();
            stop.instrumentationPointMethodDefinition.Body.SimplifyMacros();
            Instruction startingPoint = stop.instrumentationPointMethodDefinition.Body.Instructions.Last();

            Instruction callSingletonInstance =
                ilp.Create(OpCodes.Call, 
                    stop.instrumentationPointMethodDefinition.Module.Import(
                        typeof(RemoteSafeAssertionsSingleton).GetMethod("get_Instance", new Type[] { })));
            Instruction loadWpIdInstruct = ilp.Create(OpCodes.Ldc_I4, stop.GetHashCode());
            Instruction loadPortInstruct = ilp.Create(OpCodes.Ldc_I4, tempMetadata);
            Console.WriteLine("temp " + tempMetadata);

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadStopwatchField = 
                ilp.Create(
                    OpCodes.Ldfld,
                    wovenStopwatch);

            Instruction callSingletonMethod =
                ilp.Create(OpCodes.Callvirt, 
                    stop.instrumentationPointMethodDefinition.Module.Import(
                        typeof(RemoteSafeAssertionsSingleton).GetMethod(
                            "Message", new Type[] { typeof(int), typeof(int), typeof(Stopwatch) })));
           
            ilp.InsertBefore(startingPoint, callSingletonInstance);
            ilp.InsertAfter(callSingletonInstance, loadPortInstruct);
            ilp.InsertAfter(loadPortInstruct, loadWpIdInstruct);
            ilp.InsertAfter(loadWpIdInstruct, loadThis);
            ilp.InsertAfter(loadThis, loadStopwatchField);
            ilp.InsertAfter(loadStopwatchField, callSingletonMethod);

            stop.instrumentationPointMethodDefinition.Body.OptimizeMacros();*/
        }

        public void ReadAssembly(string path)
        {
            try
            {
                if (!mWeaves.ContainsKey(path))
                {
                    AssemblyDefinition m = AssemblyDefinition.ReadAssembly(path);
                    mWeaves.Add(path, m);
                }
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.AddWeavePointToNode caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }
    }
}

