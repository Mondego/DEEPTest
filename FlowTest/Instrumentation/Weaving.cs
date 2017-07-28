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
					weaveProxyMessage(
						mDefinition: module,
						targetMethod: poiMethod,
						messageToWeave: "test watch before",
						weavingMethod: new Action<MethodDefinition, List<Instruction>>(WeaveListOfInstructionsAtMethodEntry)
					);

					WeaveDebugStatementBeforeMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened before " + poi.methodOfInterest
					);
				}

				if (poi.watchAfter)
				{
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

		public static void weaveProxyMessage(
			ModuleDefinition mDefinition,
			MethodDefinition targetMethod,
			string messageToWeave,
			Delegate weavingMethod)
		{
			List<Instruction> instructionsToWeave = new List<Instruction> ();
			ILProcessor instructionProcessor = targetMethod.Body.GetILProcessor();

			WeaveInFlowTestBootstrap (
				mModule: mDefinition,
				destinationNamespace: ""
			);

			TypeDefinition bootstrapType = mDefinition.Types.Single(m => m.Name == "FlowTestBootstrap");

			instructionsToWeave.Add (
				instructionProcessor.Create (OpCodes.Ldstr, "test test test"));
			instructionsToWeave.Add (
				instructionProcessor.Create (OpCodes.Call,
					mDefinition.Import (
						bootstrapType.Methods.Single (m => m.Name == "Load"))));
			/*
			 * https://stackoverflow.com/questions/30094655/invoke-a-method-from-another-assembly
			MethodInfo mAssemblyLoadFile = typeof(System.Reflection.Assembly).GetMethod ("LoadFile", new [] { typeof(string) });
			var moduleImportAssemblyLoadFile = mDefinition.Import (mAssemblyLoadFile).Resolve ();*/

			// TODO does mDefinition already have this defined?

			//Assembly a = Assembly.LoadFile(pathToTheDll);
			// https://stackoverflow.com/questions/30094655/invoke-a-method-from-another-assembly

			/*System.Reflection.MethodInfo instanceMethod = typeof(FlowTestProxySingleton).GetMethod("get_Instance");
			System.Reflection.MethodInfo messageMethod = typeof(FlowTestProxySingleton).GetMethod("Message", new [] { typeof(string) });
			var instanceImport = mDefinition.Import(instanceMethod).Resolve();
			var messageImport = mDefinition.Import(messageMethod).Resolve();*/

			weavingMethod.DynamicInvoke(new object[] { targetMethod, instructionsToWeave});
		}

		public static void WeaveInFlowTestBootstrap(
			ModuleDefinition mModule,
			string destinationNamespace
		)
		{
			WeavingBuildingBlocks.WeavePublicStaticTypeHelper(
				module: mModule,
				typeName: "FlowTestBootstrap",
				weaveIntoNamespace: ""
			);

			TypeDefinition FlowTestBootstrapType = mModule.Types.Single(m => m.Name == "FlowTestBootstrap");

			TypeReference stringType = mModule.TypeSystem.String.Resolve ();
			stringType = mModule.Import (mModule.TypeSystem.String.Resolve ());
			MethodDefinition loader = new MethodDefinition (
				"Load",
				MethodAttributes.Public | MethodAttributes.Static,
				mModule.Import (typeof (void)));
			loader.Parameters.Add (new ParameterDefinition("payload", ParameterAttributes.None, stringType));
			FlowTestBootstrapType.Methods.Add (loader);

			loader.Body.SimplifyMacros ();
			loader.Body.Instructions.Add (
				loader.Body.GetILProcessor ().Create (OpCodes.Ldarg_0));
			loader.Body.Instructions.Add(
				loader.Body.GetILProcessor().Create(OpCodes.Call,
					mModule.Import(
						typeof (Console).GetMethod ("WriteLine", new [] { typeof (string) }))));
			loader.Body.Instructions.Add (
				loader.Body.GetILProcessor ().Create (OpCodes.Ret));
			loader.Body.OptimizeMacros ();

			WeavingBuildingBlocks.WeavePublicStaticFieldHelper(
				module: mModule,
				typeName: "FlowTestBootstrap",
				fieldName: "FlowTestApiAssembly",
				typeOfField: typeof(System.Reflection.Assembly)
			);

			MethodDefinition FlowTestBootstrapStaticConstructor = new MethodDefinition(
				".cctor",
				MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
				mModule.Import(typeof(void)));
			FlowTestBootstrapStaticConstructor.Body.Instructions.Add(
				FlowTestBootstrapStaticConstructor.Body.GetILProcessor().Create(OpCodes.Ldstr, "static constructor"));
			FlowTestBootstrapStaticConstructor.Body.Instructions.Add (
				FlowTestBootstrapStaticConstructor.Body.GetILProcessor()
				.Create (OpCodes.Call, 
					mModule.Import (
						typeof(Console).GetMethod ("WriteLine", new [] { typeof(string) })))
			);
			FlowTestBootstrapStaticConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			FlowTestBootstrapType.Methods.Add(FlowTestBootstrapStaticConstructor);
		}

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
			methodToWeave.Body.SimplifyMacros ();

			ILProcessor instructionProcessor = methodToWeave.Body.GetILProcessor();
			List<Instruction> returnInstructionsInTargetMethod = 
				methodToWeave.Body.Instructions.Where (i => i.OpCode == OpCodes.Ret).ToList ();

			foreach (Instruction returnInstruction in returnInstructionsInTargetMethod) {
				returnInstruction.Operand = listOfInstructionsToWeave [0].Operand;
				returnInstruction.OpCode = listOfInstructionsToWeave [0].OpCode;

				Instruction toInsertAfter = returnInstruction;

				for (int i = 1; i < listOfInstructionsToWeave.Count; i++) {
					instructionProcessor.InsertAfter (toInsertAfter, listOfInstructionsToWeave [i]);
					toInsertAfter = toInsertAfter.Next;
				}

				Instruction newRet = instructionProcessor.Create (OpCodes.Ret);
				instructionProcessor.InsertAfter (toInsertAfter, newRet);
			}

			methodToWeave.Body.OptimizeMacros ();
		}

		public static void WeaveAfterEveryOperandMatching(
			MethodDefinition methodToWeave,
			List<Instruction> listOfInstructionsToWeave,
			string matchOperand
		)
		{
			ILProcessor instructionProcessor = methodToWeave.Body.GetILProcessor();
			List<Instruction> matchingInstructionsInTargetMethod = 
				instructionProcessor.Body.Instructions.Where (i => i.Operand.ToString().Contains(matchOperand)).ToList ();

			Instruction[] arrayOfInstructionsToWeave = listOfInstructionsToWeave.ToArray ();
			foreach (Instruction matchingInstruction in matchingInstructionsInTargetMethod) {
				Instruction currentInstructionToWeaveAfter = matchingInstruction;
				for (int instInd = 0; instInd < arrayOfInstructionsToWeave.Length; instInd++)
				{
					instructionProcessor.InsertAfter (currentInstructionToWeaveAfter, arrayOfInstructionsToWeave[instInd]);
					currentInstructionToWeaveAfter = currentInstructionToWeaveAfter.Next;
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

		public static void DebugPrintCILInstructionsInMethod(
			MethodDefinition methodToPrint
		)
		{
		//	methodToPrint.Body.SimplifyMacros ();
			Console.WriteLine ("v---- Instructions in {0} ----v", methodToPrint.FullName);
			ILProcessor instructionProcessor = methodToPrint.Body.GetILProcessor();

			foreach(Instruction ii in instructionProcessor.Body.Instructions) {
				Console.WriteLine(ii);
			}
			Console.WriteLine ("^---- End Instructions in {0} ----^", methodToPrint.FullName);
		//	methodToPrint.Body.OptimizeMacros ();
		}

		#endregion
	}
}

