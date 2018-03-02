﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Inject;
using Mono.Cecil.Rocks;
using RemoteTestingWrapper;

namespace DeepTestFramework
{
    public class WeavingHandler
    {
        private AssemblyDefinition mPlugin;
        private Dictionary<string, AssemblyDefinition> mWeaves = 
            new Dictionary<string, AssemblyDefinition>();

        public WeavingHandler()
        {
            System.Reflection.AssemblyName pluginAssemblyMetadata =
                System.Reflection.Assembly.GetExecutingAssembly()
                    .GetReferencedAssemblies()
                    .Single(an => an.Name == "RemoteTestingWrapper");
           
            mPlugin = AssemblyDefinition.ReadAssembly(
                System.Reflection.Assembly.ReflectionOnlyLoad(
                    pluginAssemblyMetadata.FullName).Location);

        }

        public WeavePoint AddWeavePointOnEntry(
            DTNodeDefinition target,
            string nameOfWeavePointType,
            string nameOfWeavePointMethod
        )
        {
            WeavePoint entry = _generateWeavePoint(
                _target: target, 
                _nameOfWeavePointType: nameOfWeavePointType, 
                _nameOfWeavePointMethod: nameOfWeavePointMethod,
                _onEntry: true
            );

            return entry;
        }

        public WeavePoint AddWeavePointOnExit(
            DTNodeDefinition target,
            string nameOfWeavePointType,
            string nameOfWeavePointMethod
        )
        {
            WeavePoint exit = 
                _generateWeavePoint(
                    _target: target, 
                    _nameOfWeavePointType: nameOfWeavePointType, 
                    _nameOfWeavePointMethod: nameOfWeavePointMethod,
                    _onEntry: false
                ); 
            
           /*StopwatchHelper.addStopwatchInWeavePoint(
            wp,
            wp.wpMethodDefinition.Body.Instructions.First(),
            wp.wpMethodDefinition.Body.Instructions.Last().Previous
            );

            addWeavePointAssertionAnchors(wp);*/

            return exit;
        }

            
        private WeavePoint _generateWeavePoint(
            DTNodeDefinition _target,
            string _nameOfWeavePointType,
            string _nameOfWeavePointMethod,
            bool _onEntry
        )
        {
            try
            {
                AssemblyDefinition wpAssembly;
                mWeaves.TryGetValue(_target.readPath, out wpAssembly);

                // Find method to weave
                Console.WriteLine("WeavingHandler._generateWeavePoint {0}->{1}", _nameOfWeavePointType, _nameOfWeavePointMethod);
                TypeDefinition foundTypeDefinition = wpAssembly.MainModule.Types.Single(td => td.Name == _nameOfWeavePointType);
                MethodDefinition foundMethodDefinition = foundTypeDefinition.Methods.Single(md => md.Name ==  _nameOfWeavePointMethod);

                WeavePoint wp = new WeavePoint(
                    _target.readPath,
                    _nameOfWeavePointType,
                    _nameOfWeavePointMethod,
                    foundMethodDefinition
                );

                return wp;
            }

            catch (Exception e) {
                Console.WriteLine(
                    "WeavingHandler.AddWeavePoint caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }

            return null;
        }

        public void insertStopwatchAssertion(WeavePoint start, WeavePoint stop)
        {
            FieldDefinition wovenStopwatch = 
                StopwatchHelper.addStopwatchFieldToType(
                    stop.wpMethodDefinition.DeclaringType,
                    "wovenStopwatch"
                );

            StopwatchHelper.startStopwatch(
                wp: start,
                stopwatch: wovenStopwatch,
                atInstruction: start.wpMethodDefinition.Body.Instructions.First(),
                weaveBefore: true
            );
               
            StopwatchHelper.stopStopwatch(
                wp: stop,
                stopwatch: wovenStopwatch,
                atInstruction: stop.wpMethodDefinition.Body.Instructions.First(),
                weaveBefore: false
            );

            /*Console.WriteLine("----");
            foreach (Instruction i in stop.wpMethodDefinition.DeclaringType.Methods.Single(mm => mm.Name == "debugMessage").Body.Instructions) {
                Console.WriteLine(i);
            }
            Console.WriteLine("----");*/

            // TODO move this maybe but it's fine for now
            ILProcessor ilp = stop.wpMethodDefinition.Body.GetILProcessor();
            stop.wpMethodDefinition.Body.SimplifyMacros();
            Instruction startingPoint = stop.wpMethodDefinition.Body.Instructions.Last();

            Instruction callSingletonInstance =
                ilp.Create(OpCodes.Call, 
                    stop.wpMethodDefinition.Module.Import(
                        typeof(RemoteSafeAssertionsSingleton).GetMethod("get_Instance", new Type[] { })));

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);

            Instruction loadStopwatchField = 
                ilp.Create(
                    OpCodes.Ldfld,
                    wovenStopwatch);

            Instruction callSingletonMethod =
                ilp.Create(OpCodes.Callvirt, 
                    stop.wpMethodDefinition.Module.Import(
                        typeof(RemoteSafeAssertionsSingleton).GetMethod("Message", new Type[] { typeof(Stopwatch) })));
           
            ilp.InsertBefore(startingPoint, callSingletonInstance);
            ilp.InsertAfter(callSingletonInstance, loadThis);
            ilp.InsertAfter(loadThis, loadStopwatchField);
            ilp.InsertAfter(loadStopwatchField, callSingletonMethod);

            stop.wpMethodDefinition.Body.OptimizeMacros();

           /* 
            * TODO move injection stuff to safer spot
            * TypeDefinition remoteAssertionHandler = 
                mPlugin.MainModule.Types.Single(t => t.Name == "WeavingAssertionHandler");
            MethodDefinition stopwatchHandler = 
                remoteAssertionHandler.Methods.Single(m => m.Name == "stopwatchResultHook");
                 
            InjectionDefinition injector = 
                new InjectionDefinition(
                    stop.wpMethodDefinition,
                    stopwatchHandler,
                    InjectFlags.PassFields,
                    null,
                    wovenStopwatch
                );
            
            injector.Inject(
                startCode: stop.wpMethodDefinition.Body.Instructions.Last(),
                token: null,
                direction: InjectDirection.Before
            );*/
        }


        /// <summary>
        /// Adds anchor for assertions at entrance and exit of target method
        /// </summary>
        /// <param name="ad">AssemblyDefinition to be woven</param>
        /// <param name="wp">WeavePoint to use for metadata</param>
        public void addWeavePointAssertionAnchors(WeavePoint wp)
        {
            try
            {
                List<Instruction> atEntry = new List<Instruction>();
                atEntry.Add(
                    wp.wpMethodDefinition.Body.GetILProcessor().Create(
                        OpCodes.Ldstr,
                        "WeavingHandler-> addWeavePointAssertionAnchors-> Entry"
                    ));
                atEntry.Add(
                    wp.wpMethodDefinition.Body.GetILProcessor().Create(
                        OpCodes.Callvirt,
                        wp.wpMethodDefinition.Module.Import(
                            typeof(WeavingAssertionHandler).GetMethod("updateResultEntry", new [] { typeof(string) }))));
                WeavingAspectLocation.WeaveInstructionsAtEntry(
                    wp,
                    atEntry
                );

                List<Instruction> atExit = new List<Instruction>();
                atExit.Add(
                    wp.wpMethodDefinition.Body.GetILProcessor().Create(
                        OpCodes.Ldstr,
                        "WeavingHandler-> addWeavePointAssertionAnchors-> Exit"
                    ));
                atExit.Add(
                    wp.wpMethodDefinition.Body.GetILProcessor().Create(
                        OpCodes.Callvirt,
                        wp.wpMethodDefinition.Module.Import(
                            typeof(WeavingAssertionHandler).GetMethod("updateResultEntry", new [] { typeof(string) }))));
                WeavingAspectLocation.WeaveInstructionsAtExit(
                    wp,
                    atExit
                );
            }

            catch (Exception e)
            {
                Console.WriteLine("WeavingHandler.addWeavePointAnchor caught exception " + e.Message);
            }
        }

        public void WeaveDebugInfoAtWeavePointEntry(WeavePoint wp, string info = "")
        {
            List<Instruction> weaveInstructions = new List<Instruction>();
            string loadEntryValue = "[Debug] Weaving Before " + info;
        
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(OpCodes.Ldstr, loadEntryValue)
            );
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(
                    OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(string) }))));

            WeavingAspectLocation.WeaveInstructionsAtEntry(
                wp,
                weaveInstructions
            );
        }

        public void WeaveDebugInfoAtWeavePointExit(WeavePoint wp, string info = "")
        {
            List<Instruction> weaveInstructions = new List<Instruction>();
            string loadExitValue = "[Debug] Weaving After " + info;

            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(OpCodes.Ldstr, loadExitValue)
            );
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(
                    OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(string) }))));

            WeavingAspectLocation.WeaveInstructionsAtExit(
                wp,
                weaveInstructions
            );
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

        public void Write(DTNodeDefinition target, string alternateWritePath = "")
        {
            try
            {
                string writePath = target.readPath;
                if (alternateWritePath != "")
                {
                    writePath = alternateWritePath;
                }

                AssemblyDefinition assemblyToWrite = mWeaves[target.readPath];
                assemblyToWrite.Write(writePath);
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.Write() caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }
    }
}
