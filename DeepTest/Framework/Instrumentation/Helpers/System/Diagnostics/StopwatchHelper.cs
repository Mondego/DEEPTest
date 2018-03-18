using System;
using System.Diagnostics;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DeepTestFramework
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
        public static FieldDefinition addStopwatchFieldToType(
            TypeDefinition t, 
            string fieldName
        )
        {
            FieldDefinition stopwatch =
                new FieldDefinition(
                    fieldName,
                    FieldAttributes.Private,
                    t.Module.Import(typeof(Stopwatch))
                );
            
            t.Fields.Add(stopwatch);

            return stopwatch;
        }

        public static void startStopwatch(
            InstrumentationPoint wp, 
            FieldDefinition stopwatch, 
            Instruction atInstruction, 
            bool weaveBefore
        )
        {
            wp.instrumentationPointMethodDefinition.Body.SimplifyMacros();
            ILProcessor ilp = wp.instrumentationPointMethodDefinition.Body.GetILProcessor();

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction callStopwatchStartNew = ilp.Create(
                    OpCodes.Call,
                wp.instrumentationPointMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("StartNew", new Type[] { })));
            Instruction storeStopwatchInFieldDef = ilp.Create(
                    OpCodes.Stfld,
                    stopwatch
                );

            if (weaveBefore) {
                ilp.InsertBefore(atInstruction, loadThis);
                ilp.InsertAfter(loadThis, callStopwatchStartNew);
                ilp.InsertAfter(callStopwatchStartNew, storeStopwatchInFieldDef);
            } else {
                ilp.InsertAfter(atInstruction, loadThis);
                ilp.InsertAfter(loadThis, callStopwatchStartNew);
                ilp.InsertAfter(callStopwatchStartNew, storeStopwatchInFieldDef);
            }

            wp.instrumentationPointMethodDefinition.Body.OptimizeMacros();
        }

        public static void stopStopwatch(
            InstrumentationPoint wp, 
            FieldDefinition stopwatch, 
            Instruction atInstruction, 
            bool weaveBefore
        )
        {
            wp.instrumentationPointMethodDefinition.Body.SimplifyMacros();
            ILProcessor ilp = wp.instrumentationPointMethodDefinition.Body.GetILProcessor();   

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadStopwatchField = 
                ilp.Create(
                    OpCodes.Ldfld,
                    stopwatch);
            Instruction callStop = 
                ilp.Create(OpCodes.Callvirt, 
                    wp.instrumentationPointMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("Stop", new Type[] { })));

            if (weaveBefore) {
                ilp.InsertBefore(atInstruction, loadThis);
                ilp.InsertAfter(loadThis, loadStopwatchField);
                ilp.InsertAfter(loadStopwatchField, callStop);
            } else {
                ilp.InsertAfter(atInstruction, loadThis);
                ilp.InsertAfter(loadThis, loadStopwatchField);
                ilp.InsertAfter(loadStopwatchField, callStop);
            }
           
            wp.instrumentationPointMethodDefinition.Body.OptimizeMacros();
        }
    }
}

