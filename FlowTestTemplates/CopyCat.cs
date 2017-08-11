using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Collections.Generic;

namespace FlowTestTemplates
{
	public static class CopyCat
	{
		// WIP
		public static void copyMethod(
			MethodDefinition copy,
			MethodDefinition paste
		)
		{
			paste.Body.SimplifyMacros();
			foreach (Instruction i in copy.Body.Instructions) {
				paste.Body.Instructions.Add(copyInstruction(i, copy));
			}
			paste.Body.OptimizeMacros();
		}

		// WIP
		public static void shallowCopyTypeStructure(
			TypeDefinition copy,
			MethodDefinition paste
		)
		{
			foreach (FieldDefinition fd in copy.Fields) {

			}

			foreach (MethodDefinition md in copy.Methods) {
			
			}

			foreach (PropertyDefinition pd in copy.Properties) {

			}
		}
			
		// Branching will be a pain
		// TODO, how Cecil does this
		// https://github.com/jbevain/cecil/blob/master/Mono.Cecil.Cil/CodeReader.cs
		public static Instruction copyInstruction(
			Instruction instruction, 
			MethodDefinition sourceMethod
		)
		{
			switch (instruction.OpCode.OperandType)
			{
			case OperandType.InlineNone:
				return Instruction.Create(instruction.OpCode);
				break;

			default:
				Console.WriteLine(instruction.OpCode.OperandType + " no match found: " + instruction);
				break;
			}

			return Instruction.Create(OpCodes.Nop);
		}
	}
}

