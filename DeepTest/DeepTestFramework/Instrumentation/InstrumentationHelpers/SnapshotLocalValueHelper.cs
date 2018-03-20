using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using RemoteTestingWrapper;

namespace DeepTestFramework
{
    public class SnapshotLocalValueHelper : InstrumentationHelper
    {
        protected string snapshotFieldName;

        public SnapshotLocalValueHelper(InstrumentationAPI i, string fieldName) : base(i)
        {
            snapshotFieldName = fieldName;
        }

        protected override void InstrumentationHelperInitialization(
            InstrumentationPoint ip
        )
        {
            // TODO will likely need more initialization to retrieve a value on demand
            base.InstrumentationHelperInitialization(ip);
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            ILProcessor ilp = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();
            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();
    
            // Load value of interest and box as object
            FieldDefinition snapshotFieldDefinition = 
                ip.instrumentationPointTypeDefinition.Fields
                    .Single(f => f.Name == snapshotFieldName);
            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadSnapshotValue =
                ilp.Create(OpCodes.Ldfld, snapshotFieldDefinition);
            Instruction boxSnapshotField =
                ilp.Create(OpCodes.Box, snapshotFieldDefinition.FieldType);
            
            // Load IP name
            Instruction loadIpId = ilp.Create(OpCodes.Ldstr, ip.Name);

            // Call default remote test wrapper handler
            Instruction loadRemoteTestDriverInstance =
                ilp.Create(OpCodes.Call, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod("get_Instance", new Type[] { })));
           
            Instruction callCaptureIpContents =
                ilp.Create(OpCodes.Callvirt, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod(
                            "CaptureInstrumentationPoint", 
                            new Type[] { typeof(object), typeof(string) })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();
           
            weaveOpeningInstructions.Add(loadRemoteTestDriverInstance);
            weaveOpeningInstructions.Add(loadThis);
            weaveOpeningInstructions.Add(loadSnapshotValue);
            weaveOpeningInstructions.Add(boxSnapshotField);
            weaveOpeningInstructions.Add(loadIpId);
            weaveOpeningInstructions.Add(callCaptureIpContents);

            return weaveOpeningInstructions;
        }
    }
}

