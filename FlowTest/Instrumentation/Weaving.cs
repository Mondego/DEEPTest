using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace FlowTest
{
	public class Weaving
	{
		#region Points of interest
		public static void WeavePointofInterest(
			ModuleDefinition module, 
			FlowTestPointOfInterest poi
		)
		{
			try {
				TypeDefinition destinationType = module.Types.Single(t => t.Name == poi.parentObjectOfWatchpoint);
				MethodDefinition poiMethod = destinationType.Methods.Single(m => m.Name == poi.methodOfInterest);
				ILProcessor instructionProcessor = poiMethod.Body.GetILProcessor();

				if (poi.watchBefore)
				{
					/*WeavingFlowTestProxy.InvokeResultAggregatorBeforeMethod(
						method: poiMethod,
						value: "poi.watchbefore"
					);*/

					WeaveDebugStatementBeforeMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened before " + poi.methodOfInterest
					);
				}

				if (poi.watchAfter)
				{
					/*WeavingFlowTestProxy.InvokeResultAggregatorAfterMethod(
						method: poiMethod,
						value: "poi.watchafter"
					);*/

					WeaveDebugStatementAfterMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened after " + poi.methodOfInterest
					);
				}
			}

			catch (Exception e) {
				Console.WriteLine ("| FlowTest Weaver caught an exception while adding a point of interest.");
				Console.WriteLine ("| PoI: {0} => {1}", poi.parentObjectOfWatchpoint, poi.methodOfInterest);
				Console.WriteLine ("| {0} {1}", e.InnerException, e.Message);
			}
		}
		#endregion

		#region FlowTestProxySingleton



		#endregion

		#region Low-level weaving

		//IL_0001: ldc.i4 N_MILLISECONDS
		//IL_0006: call System.Void System.Threading.Thread::Sleep(System.Int32)
		public static void WeaveThreadSleepAfterEachMatch(
			MethodDefinition targetMethod,
			int nMilliseconds,
			string matchOperand
		)
		{
			List<Instruction> instructionsToWeave = new List<Instruction> ();
			ILProcessor instructionProcessor = targetMethod.Body.GetILProcessor();

			Instruction loadMillisecondsInstruction = instructionProcessor.Create(OpCodes.Ldc_I4, nMilliseconds);
			instructionsToWeave.Add (loadMillisecondsInstruction);

			Instruction sleepInstruction = 
				instructionProcessor.Create(OpCodes.Call, 
					targetMethod.Module.Import(
						typeof (System.Threading.Thread).GetMethod ("Sleep", new [] { typeof (int) })));
			instructionsToWeave.Add (sleepInstruction);

			List<Instruction> matchingOperandsInstructions = new List<Instruction> ();
			foreach (Instruction i in targetMethod.Body.Instructions) {
				if (i.Operand != null) {
					if (((string)i.Operand.ToString ()).Contains (matchOperand)) {
						matchingOperandsInstructions.Add (i);
					}
				}
			}

			//Instruction[] arrayOfInstructionsToWeave = instructionsToWeave.ToArray();
			foreach (Instruction matchingInstruction in matchingOperandsInstructions) {
				Instruction toWeaveBefore = matchingInstruction.Next;
				foreach (Instruction toWeave in instructionsToWeave) {
					instructionProcessor.InsertBefore (toWeaveBefore, toWeave);
				}
			}
		}



		#endregion
	
		#region Nifty tools for debugging the weaver

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

		#endregion
	}
}

