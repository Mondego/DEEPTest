using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DeepTest
{
    public class WeavingAspectLocation
    {
        public static void WeaveInstructionsAtExit(
            WeavePoint wpAfter,
            List<Instruction> instructionsToWeave
        )
        {
            ILProcessor instructionProcessor = wpAfter.wpMethodDefinition.Body.GetILProcessor();
            List<Instruction> returnInstructionsInTargetMethod = 
                wpAfter.wpMethodDefinition.Body.Instructions.Where (i => i.OpCode == OpCodes.Ret).ToList();

            foreach (Instruction returnInstruction in returnInstructionsInTargetMethod) {
                foreach (Instruction weaveInstruction in instructionsToWeave) {
                    instructionProcessor.InsertBefore (returnInstruction, weaveInstruction);
                }
            }
        }

        public static void WeaveInstructionsAtEntry(
            WeavePoint wpBefore,
            List<Instruction> instructionsToWeave
        )
        {
            Instruction originalFirstInstruction = wpBefore.wpMethodDefinition.Body.Instructions.First();
            ILProcessor instructionProcessor = wpBefore.wpMethodDefinition.Body.GetILProcessor();

            foreach (Instruction weaveInstruction in instructionsToWeave) {
                instructionProcessor.InsertBefore (originalFirstInstruction, weaveInstruction);
            }
        }
    }
}

