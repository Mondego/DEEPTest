using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using RemoteTestingWrapper;

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

            // Stop stopwatch
            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadStopwatchField = ilp.Create(OpCodes.Ldfld, mStopwatchDefinition);
            Instruction callStop = 
                ilp.Create(OpCodes.Callvirt, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(Stopwatch).GetMethod("Stop", new Type[] { })));

            // Load stopwatch, and ip id again
            Instruction loadStopwatchForCapture = ilp.Create(OpCodes.Ldarg_0);
            Instruction reloadStopwatchField = ilp.Create(OpCodes.Ldfld, mStopwatchDefinition);

            // Load instrumentation point id
            Instruction loadIpName = ilp.Create(OpCodes.Ldstr, ip.Name);

            // Load and call wrapper
            Instruction LoadRemoteTestDriverInstance =
                ilp.Create(OpCodes.Call, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod("get_Instance", new Type[] { })));

            Instruction callCaptureIpContents =
                ilp.Create(OpCodes.Callvirt, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod(
                            "CaptureStopwatchEndPoint", 
                            new Type[] { typeof(Stopwatch), typeof(string) })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveClosingInstructions.Add(loadThis);
            weaveClosingInstructions.Add(loadStopwatchField);
            weaveClosingInstructions.Add(callStop);

            weaveClosingInstructions.Add(LoadRemoteTestDriverInstance);
            weaveClosingInstructions.Add(loadStopwatchForCapture);
            weaveClosingInstructions.Add(reloadStopwatchField);
            weaveClosingInstructions.Add(loadIpName);
            weaveClosingInstructions.Add(callCaptureIpContents);

            return weaveClosingInstructions;
        }
    }
}

