using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

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

            string loadEntryValue = "[Start Snapshot] Field: " + snapshotFieldName;
            string endSnapshotValue = "[End Snapshot Field]";
            FieldDefinition snapshotFieldDefinition = 
                ip.instrumentationPointTypeDefinition.Fields
                    .Single(f => f.Name == snapshotFieldName);

            Instruction startSnapshotListing = ilp.Create(OpCodes.Ldstr, loadEntryValue);
            Instruction endSnapshotListing = ilp.Create(OpCodes.Ldstr, endSnapshotValue);
            Instruction systemConsoleWriteString =
                ilp.Create(
                    OpCodes.Call,
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(string) })));

            Instruction loadThis = ilp.Create(OpCodes.Ldarg_0);
            Instruction loadSnapshotValue =
                ilp.Create(OpCodes.Ldfld, snapshotFieldDefinition);
            Instruction boxSnapshotField =
                ilp.Create(OpCodes.Box, snapshotFieldDefinition.FieldType);
            Instruction systemConsoleWriteObject =
                ilp.Create(
                    OpCodes.Call,
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(object) })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveOpeningInstructions.Add(startSnapshotListing);
            weaveOpeningInstructions.Add(systemConsoleWriteString);
            weaveOpeningInstructions.Add(loadThis);
            weaveOpeningInstructions.Add(loadSnapshotValue);
            weaveOpeningInstructions.Add(boxSnapshotField);
            weaveOpeningInstructions.Add(systemConsoleWriteObject);
            weaveOpeningInstructions.Add(endSnapshotListing);
            weaveOpeningInstructions.Add(systemConsoleWriteString);

            return weaveOpeningInstructions;
        }
    }
}

