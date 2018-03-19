using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;

namespace DeepTestFramework
{
    public static class InstrumentationPositionInMethodHelper
    {
        // TODO make something more elegant than just passing a list
        // TODO improved error checking
        public static void WeaveInstructionsAtMethodExit(
            InstrumentationPoint ip,
            List<Instruction> instructionsToWeave
        )
        {
            if (instructionsToWeave.Count == 0) {
                return;
            }

            List<Instruction> findReturnInstructionsInTargetMethod = 
                ip.instrumentationPointMethodDefinition.Body.Instructions
                    .Where(i => i.OpCode == OpCodes.Ret).ToList();

            ILProcessor instructionProcessor = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();
            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();

            // TODO need more elegant way to resolve this
            foreach (Instruction returnInstruction in findReturnInstructionsInTargetMethod) {
                foreach (Instruction weaveInstruction in instructionsToWeave) {
                    instructionProcessor.InsertBefore (returnInstruction, weaveInstruction);
                }
            }

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();
        }

        // TODO make something more elegant than just passing a list
        // TODO improved error checking
        public static void WeaveInstructionsAtMethodEntry(
            InstrumentationPoint ip,
            List<Instruction> instructionsToWeave
        )
        {
            if (instructionsToWeave.Count == 0) {
                return;
            }

            ILProcessor instructionProcessor = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();
            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();
            Instruction originalFirstInstruction = ip.instrumentationPointMethodDefinition.Body.Instructions.First();

            foreach (Instruction weaveInstruction in instructionsToWeave) {
                instructionProcessor.InsertBefore (originalFirstInstruction, weaveInstruction);
            }

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();
        }
    }
}

