using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DeepTestFramework
{
    public class StopwatchHelper : InstrumentationHelper
    {
        private FieldDefinition mStopwatchDefinition; 

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
        public StopwatchHelper(InstrumentationAPI i) : base(i)
        {
        }

        protected override void InstrumentationHelperInitialization(
            InstrumentationPoint ip
        )
        {
            // TODO handle case of multiple stopwatch, need a naming policy
            string fieldName = "stopwatch_" + ip.Name;

            mStopwatchDefinition =
                new FieldDefinition(
                    fieldName,
                    FieldAttributes.Private,
                    ip.instrumentationPointTypeDefinition.Module.Import(typeof(Stopwatch))
                );
            
            ip.instrumentationPointTypeDefinition.Fields.Add(mStopwatchDefinition);
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            // TODO Make sure to error check for valid stopwatch field definition
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            ILProcessor ilp = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();
            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction callStopwatchStartNew = ilp.Create(
                OpCodes.Call,
                ip.instrumentationPointMethodDefinition.Module.Import(
                    typeof(Stopwatch).GetMethod("StartNew", new Type[] { })));
            Instruction storeStopwatchInFieldDef = ilp.Create(
                OpCodes.Stfld,
                mStopwatchDefinition
            );
            
            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveOpeningInstructions.Add(loadThis);
            weaveOpeningInstructions.Add(callStopwatchStartNew);
            weaveOpeningInstructions.Add(storeStopwatchInFieldDef);

            return weaveOpeningInstructions;
        }

        protected override List<Instruction> InstrumentationHelperClosingInstructions(
            InstrumentationPoint ip
        )
        {
            // TODO Make sure to error check for valid stopwatch field definition
            List<Instruction> weaveClosingInstructions = new List<Instruction>();

            ILProcessor ilp = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();
            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadStopwatchField = ilp.Create(OpCodes.Ldfld, mStopwatchDefinition);
            Instruction callStop = 
                ilp.Create(OpCodes.Callvirt, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("Stop", new Type[] { })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveClosingInstructions.Add(loadThis);
            weaveClosingInstructions.Add(loadStopwatchField);
            weaveClosingInstructions.Add(callStop);

            return weaveClosingInstructions;
        }
    }
}

