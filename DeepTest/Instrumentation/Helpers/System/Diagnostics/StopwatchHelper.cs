using System;
using System.Diagnostics;

using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DeepTest
{
    /// <summary>
    /// Helper for commonly used System.Diagnostics.Stopwatch functionality in DT.
    /// Not a feature complete copy of the Stopwatch class.
    /// 
    /// Weaves as follows:
    /// var sw = Stopwatch.StartNew();
    /// sw.Stop();
    /// do something with sw.ElapsedMilliseconds
    /// 
    /// sw.ElapsedMilliseconds is a wholesome choice here since it adjusts for machine/OS-specific timing.
    /// </summary>
    public class StopwatchHelper
    {
        /// <summary>
        /// Adds the stopwatch in weave point that initiates before a given instruction, and stops after another.
        /// </summary>
        /// <param name="wp">WeavePoint wp</param>
        /// <param name="startBefore"></param>
        /// <param name="stopAfter"></param>
        public static void addStopwatchInWeavePoint(
            WeavePoint wp, 
            Instruction startBefore, 
            Instruction stopAfter
        )
        {
            wp.wpMethodDefinition.Body.SimplifyMacros();
            ILProcessor ilp = wp.wpMethodDefinition.Body.GetILProcessor();

            // Step 1: Add variable to method body
            VariableDefinition stopwatchVariableDefinition = 
                new VariableDefinition(
                    "sw", 
                    wp.wpMethodDefinition.Module.Import(typeof(Stopwatch)));
            wp.wpMethodDefinition.Body.Variables.Add(stopwatchVariableDefinition);

            // Step 2: Initialize before target instruction
            // 
            // var sw = Stopwatch.StartNew();
            //
            // instruction: call System.Diagnostics.Stopwatch System.Diagnostics.Stopwatch::StartNew()
            // instruction: stloc.1
            Instruction callStopwatchStartNewInstruction = 
                ilp.Create(
                    OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                                typeof(Stopwatch).GetMethod("StartNew", new Type[] {})));
            Instruction storeStopwatchInstruction = ilp.Create(OpCodes.Stloc, stopwatchVariableDefinition);
            ilp.InsertBefore(startBefore, callStopwatchStartNewInstruction);
            ilp.InsertBefore(startBefore, storeStopwatchInstruction);

            // Step 3: Stop after second target instruction
            // 
            // sw.Stop();
            // 
            // instruction: ldloc.1
            // instruction: callvirt System.Void System.Diagnostics.Stopwatch::Stop()
            Instruction loadStopwatchInstruction = ilp.Create(OpCodes.Ldloc, stopwatchVariableDefinition);
            Instruction callVirtualStopwatchStop =
                ilp.Create(OpCodes.Callvirt, 
                    wp.wpMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("Stop", new Type[] {})));
                    
            ilp.InsertAfter(stopAfter, loadStopwatchInstruction);
            ilp.InsertAfter(loadStopwatchInstruction, callVirtualStopwatchStop);

            // Step 4 --- TODO move to another function?
            //
            // sw.ElapsedMilliseconds
            // 
            // instruction: ldloc.1
            // instruction: callvirt System.Int64 System.Diagnostics.Stopwatch::get_ElapsedMilliseconds()
            // instruction: call System.Void System.Console::WriteLine(System.Int64)
            Instruction loadStopwatchInstruction2 = ilp.Create(OpCodes.Ldloc, stopwatchVariableDefinition);
            Instruction callVirtualStopwatchGetElapsedMilliseconds =
                ilp.Create(OpCodes.Callvirt,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("get_ElapsedMilliseconds", new Type[] { })));
            Instruction printValue =
                ilp.Create(OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(Console).GetMethod("WriteLine", new [] { typeof(Int64) })));

            ilp.InsertAfter(callVirtualStopwatchStop,loadStopwatchInstruction2);
            ilp.InsertAfter(loadStopwatchInstruction2,callVirtualStopwatchGetElapsedMilliseconds);
            ilp.InsertAfter(callVirtualStopwatchGetElapsedMilliseconds,printValue);
            wp.wpMethodDefinition.Body.OptimizeMacros();
        }
    }
}

