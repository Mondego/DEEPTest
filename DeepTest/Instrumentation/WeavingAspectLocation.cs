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
        [Flags]
        public enum WeaveLocation
        {
            MethodEntry = 1,
            MethodExit = 2,
            //MethodRegex
        }

        /// <summary>
        /// Weaves the instructions.
        /// </summary>
        /// <param name="position">Takes enum, currently limited to entry/exit</param>
        /// <param name="listOfInstructionsToWeave">Instructions</param>
        public static void WeaveInstructions(
            WeaveLocation position, 
            WeavePoint wp,
            List<Instruction> listOfInstructionsToWeave)
        {
            try
            {
                wp.wpMethodDefinition.Body.SimplifyMacros ();
               
            
                if (position.HasFlag(WeaveLocation.MethodEntry))
                {
                    WeaveInstructionsAtEntry(wp, listOfInstructionsToWeave);
                }
            
                if (position.HasFlag(WeaveLocation.MethodExit))
                {
                    WeaveInstructionsAtExit(wp, listOfInstructionsToWeave);
                }

                wp.wpMethodDefinition.Body.OptimizeMacros ();
            }

            catch (Exception e)
            {
                Console.WriteLine("WeavingAspectLocation.WeaveInstructions caught exception {0}", e.Message);
            }
        }

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

