using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace FlowTestAPI
{
	public class WeavingAtLocation
	{
		// TODO Some of this behavior should probably happen at the FlowTestRuntime level, 
		// and just pass in a payload to send off as needed.
		public static void WeaveModuleAtTargetPointCall(ModuleDefinition module, FlowTestPointOfInterest poi)
		{
			try {
				TypeDefinition poiParentType = module.Types.Single(t => t.Name == poi.parentObjectOfWatchpoint);
				MethodDefinition poiMethod = poiParentType.Methods.Single(m => m.Name == poi.methodOfInterest);
				ILProcessor instructionProcessor = poiMethod.Body.GetILProcessor();

				/*Console.WriteLine("...");
				foreach(Instruction ii in instructionProcessor.Body.Instructions) {
					Console.WriteLine(ii);
				}
				Console.WriteLine("...");*/

				if (poi.watchBefore)
				{
					WeavingCustomFields.InvokeMethodOfPublicCustomField(
						destinationModule : module,
						destinationTypeName: poi.parentObjectOfWatchpoint,
						destinationMethodName: poi.methodOfInterest,

						// TODO need to make this part of the flowtest runtime so poi can access it not like this
						customFieldTypeName: "MainClass",
						customFieldName: "mWovenMessagesHandler",
						customFieldMethodToInvoke: "SendRunTimeEvent",
						customFieldType: typeof(FlowTestAwayTeam),

						invokedMethodArgTypes: new Type[] { typeof(string) },// new Type[] { typeof(FlowTestInstrumentationEvent) },
						invokedMethodArgs: new string[] { poi.generatePayloadString("before") }, // { poi.generatePayload("after") },
						weavePositionIsStart: true
					);

					WeaveDebugStatementBeforeMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened before " + poi.methodOfInterest
					);
				}

				if (poi.watchAfter)
				{
					WeavingCustomFields.InvokeMethodOfPublicCustomField(
						destinationModule : module,
						destinationTypeName: poi.parentObjectOfWatchpoint,
						destinationMethodName: poi.methodOfInterest,

						// TODO need to make this part of the flowtest runtime so poi can access it not like this
						customFieldTypeName: "MainClass",
						customFieldName: "mWovenMessagesHandler",
						customFieldMethodToInvoke: "SendRunTimeEvent",
						customFieldType: typeof(FlowTestAwayTeam),

						invokedMethodArgTypes: new Type[] { typeof(string) },// new Type[] { typeof(FlowTestInstrumentationEvent) },
						invokedMethodArgs: new string[] { poi.generatePayloadString("after") }, // { poi.generatePayload("after") },
						weavePositionIsStart: false
					);

					WeaveDebugStatementAfterMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened after " + poi.methodOfInterest
					);
				}
			}

			catch (Exception e) {
				Console.WriteLine ("FlowTest Weaver caught an exception while adding a point of interest.");
				Console.WriteLine ("PoI: {0} => {1}", poi.parentObjectOfWatchpoint, poi.methodOfInterest);
				Console.WriteLine ("{0} {1}", e.InnerException, e.Message);
			}
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

			WeaveListOfInstructionsAtMethodEntry (
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

			WeaveListOfInstructionsAtMethodExit (
				methodToWeave : targetMethod,
				listOfInstructionsToWeave : statementsToWeave
			);
		}

		/////////////////////////

		public static void WeaveListOfInstructionsAtMethodEntry(
			MethodDefinition methodToWeave,
			List<Instruction> listOfInstructionsToWeave
		)
		{
			ILProcessor instructionProcessor = methodToWeave.Body.GetILProcessor();
			Instruction originalFirstInstruction = methodToWeave.Body.Instructions.First ();

			foreach (Instruction weaveInstruction in listOfInstructionsToWeave) {
				instructionProcessor.InsertBefore (originalFirstInstruction, weaveInstruction);
			}
		}

		public static void WeaveListOfInstructionsAtMethodExit(
			MethodDefinition methodToWeave,
			List<Instruction> listOfInstructionsToWeave
		)
		{
			ILProcessor instructionProcessor = methodToWeave.Body.GetILProcessor();
			List<Instruction> returnInstructionsInTargetMehod = 
				instructionProcessor.Body.Instructions.Where (i => i.OpCode == OpCodes.Ret).ToList ();

			foreach (Instruction returnInstruction in returnInstructionsInTargetMehod) {
				foreach (Instruction weaveInstruction in listOfInstructionsToWeave) {
					instructionProcessor.InsertBefore (returnInstruction, weaveInstruction);
				}
			}
		}
	}
}

