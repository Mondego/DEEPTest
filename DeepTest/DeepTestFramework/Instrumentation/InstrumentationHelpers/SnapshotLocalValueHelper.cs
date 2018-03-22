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
        private FieldDefinition mSnapshotFieldDefinition;

        public SnapshotLocalValueHelper(InstrumentationAPI i, string fieldName) : base(i)
        {
            snapshotFieldName = fieldName;
        }

        protected override void InstrumentationHelperInitialization(
            InstrumentationPoint ip
        )
        {
            // TODO --- need a more graceful way to name these
            mSnapshotFieldDefinition =
                new FieldDefinition(
                    "snapshot_" + ip.Name,
                    FieldAttributes.Public,
                    ip.instrumentationPointTypeDefinition.Module.Import(typeof(WovenSnapshot))
                );
            
            ip.instrumentationPointTypeDefinition.Fields.Add(mSnapshotFieldDefinition);

            // TODO might want a special case of this for constructors, static classes/constructors get messy
            List<Instruction> wovenFieldInitializationInstructions = new List<Instruction>();
            MethodDefinition ipParentTypeConstructor = ip.instrumentationPointTypeDefinition.Methods.Single(m => m.Name == ".ctor");
            FieldDefinition snapshotTargetField = 
                ip.instrumentationPointTypeDefinition.Fields
                    .Single(f => f.Name == snapshotFieldName);
            ILProcessor ctorIlp = ipParentTypeConstructor.Body.GetILProcessor();

            // Load self
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Ldarg_0)
            );

            // Load arguments to WovenSnapshot constructor: IP Name
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Ldstr, ip.Name)
            );

            // Load arguments to WovenSnapshot constructor: Target field
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Ldarg_0)
            );
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Ldfld, snapshotTargetField)
            );
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Box, snapshotTargetField.FieldType)
            );

            // Create with constructor call and store into field
            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(
                    OpCodes.Newobj,
                    ipParentTypeConstructor.Module.Import(
                        typeof(WovenSnapshot).GetConstructor(new Type[] { typeof(string), typeof(object) }))
                )
            );

            wovenFieldInitializationInstructions.Add(
                ctorIlp.Create(OpCodes.Stfld, mSnapshotFieldDefinition)
            );

            InstrumentationPositionInMethodHelper. WeaveInstructionsAtMethodExit(ipParentTypeConstructor, wovenFieldInitializationInstructions); 
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

