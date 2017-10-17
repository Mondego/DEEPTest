using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Mono.Cecil.Rocks;

namespace FlowTest
{
	public class WeavingDebug
	{
		public static void PrintCILInstructionsInMethod(
			MethodDefinition methodToPrint
		)
		{
			Console.WriteLine ("v---- Instructions in {0} ----v", methodToPrint.FullName);
			Console.WriteLine("v---- Has {0} variables ", methodToPrint.Body.Variables.Count);
			ILProcessor instructionProcessor = methodToPrint.Body.GetILProcessor();

			foreach(Instruction ii in instructionProcessor.Body.Instructions) {
				Console.WriteLine(ii);
			}
			Console.WriteLine ("^---- End Instructions in {0} ----^", methodToPrint.FullName);
		}

		public static void WeaveDebugStatementBeforeMethod(
			MethodDefinition targetMethod,
			string printDebugValue
		)
		{
			List<Instruction> statementsToWeave = new List<Instruction> ();
			ILProcessor instructionProcessor = targetMethod.Body.GetILProcessor();

			Instruction loadStringInstruction = instructionProcessor.Create(OpCodes.Ldstr, printDebugValue);
			statementsToWeave.Add (loadStringInstruction);

			Instruction writeValueToConsoleInstruction = 
				instructionProcessor.Create(OpCodes.Call, 
					targetMethod.Module.Import(
						typeof (Console).GetMethod ("WriteLine", new [] { typeof (string) })));
			statementsToWeave.Add (writeValueToConsoleInstruction);

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodEntry (
				methodToWeave : targetMethod,
				listOfInstructionsToWeave : statementsToWeave
			);
		}

		public static void WeaveDebugStatementAfterMethod(
			MethodDefinition targetMethod,
			string printDebugValue
		)
		{
			List<Instruction> statementsToWeave = new List<Instruction> ();
			ILProcessor instructionProcessor = targetMethod.Body.GetILProcessor();

			Instruction loadStringInstruction = instructionProcessor.Create(OpCodes.Ldstr, printDebugValue);
			statementsToWeave.Add (loadStringInstruction);

			Instruction writeValueToConsoleInstruction = 
				instructionProcessor.Create(OpCodes.Call, 
					targetMethod.Module.Import(
						typeof (Console).GetMethod ("WriteLine", new [] { typeof (string) })));
			statementsToWeave.Add (writeValueToConsoleInstruction);

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodExit (
				methodToWeave : targetMethod,
				listOfInstructionsToWeave : statementsToWeave
			);
		}
	
		public static void weaveGenericMethodBody(
			MethodDefinition targetMethod
		)
		{
			string placeholder = "Placeholder for " + targetMethod.Name;
			TypeReference voidType = targetMethod.Module.Import(typeof(void));

			targetMethod.Body.Instructions.Clear();
			targetMethod.Body.SimplifyMacros();

			targetMethod.Body.Instructions.Add(
				targetMethod.Body.GetILProcessor().Create(
					OpCodes.Ldstr, placeholder));
			targetMethod.Body.Instructions.Add(
				targetMethod.Body.GetILProcessor().Create(
					OpCodes.Call,
					targetMethod.Module.Import(
						typeof(Console).GetMethod("WriteLine", new [] { typeof(string) }))));

			if (targetMethod.ReturnType != voidType) {
				targetMethod.Body.Instructions.Add(
					targetMethod.Body.GetILProcessor().Create(
						OpCodes.Ldnull));
			}

			targetMethod.Body.Instructions.Add(
				targetMethod.Body.GetILProcessor().Create(
					OpCodes.Ret));

			targetMethod.Body.OptimizeMacros();

			PrintCILInstructionsInMethod(targetMethod);
		}
	}
}

