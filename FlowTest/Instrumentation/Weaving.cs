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
				string destinationFTplaceholder = GetStandaloneFTFieldName(poi.parentObjectOfWatchpoint);

				//WeavingDebugTools.ConsoleWriteEachCILInstruction(poiMethod);

				// Weaving in field into this class
				FieldDefinition ftRuntimeHook = 
					destinationType.Fields.SingleOrDefault(fd => fd.FullName == GetStandaloneFTFieldName(poi.parentObjectOfWatchpoint));
				

				if (ftRuntimeHook == null)
				{
					TypeReference FTAwayTeamType = module.Import(typeof(FlowTestAwayTeam).GetType());

					FieldDefinition wovenFieldDefinition = new FieldDefinition (
						destinationFTplaceholder,
						Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
						FTAwayTeamType
					);

					destinationType.Fields.Add(wovenFieldDefinition);
		
					foreach (MethodDefinition md in destinationType.Methods.Where(m => m.Name == ".ctor"))
					{
						Console.WriteLine("m: " + md);
						/*WeavingCustomFields.InitializeCustomField (
							destinationModule: module,
							destinationClassName: poi.parentObjectOfWatchpoint,
							destinationMethodName: ".ctor",

							customFieldName: WeavingDebugTools.GetStandaloneFTFieldName(poi.parentObjectOfWatchpoint),
							customFieldAttributes: Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
							customFieldType: typeof(FlowTestAwayTeam),
							customFieldConstructorArgTypes: new Type[] { typeof(int), typeof(int) },
							customFieldConstructorArgs: new object[] { 60011, 60012 }
						);*/
					}
				}

				if (poi.watchBefore)
				{
					InvokeMethodOfPublicCustomField(
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
					
					/*WeavingCustomFields.InvokeMethodOfPublicCustomField(
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
					);*/

					WeaveDebugStatementAfterMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened after " + poi.methodOfInterest
					);
				}

				//WeavingDebugTools.ConsoleWriteEachCILInstruction(poiMethod);
			}

			catch (Exception e) {
				Console.WriteLine ("FlowTest Weaver caught an exception while adding a point of interest.");
				Console.WriteLine ("PoI: {0} => {1}", poi.parentObjectOfWatchpoint, poi.methodOfInterest);
				Console.WriteLine ("{0} {1}", e.InnerException, e.Message);
			}
		}
		#endregion

		#region Fields

		public static void WeaveCustomFieldIntoClass (
			ModuleDefinition m,
			string customFieldName,
			FieldAttributes customFieldAttributes,
			Type customFieldType,
			string destinationClassName
		)
		{
			try
			{
				FieldDefinition wovenFieldDefinition =
					new FieldDefinition (
						customFieldName,
						customFieldAttributes,
						m.Import (customFieldType));

				TypeDefinition destinationClassType = m.Types.Single (t => t.Name == destinationClassName);
				destinationClassType.Fields.Add(wovenFieldDefinition);
			}
			catch (Exception e) {
				Console.WriteLine("FlowTest custom field weaver caught exception " + e.Message);
			}
		}

		public static void InitializeCustomField (
			ModuleDefinition destinationModule,
			string destinationClassName,
			string destinationMethodName,
			string customFieldName,
			FieldAttributes customFieldAttributes,
			Type customFieldType,
			Type[] customFieldConstructorArgTypes,
			object[] customFieldConstructorArgs
		)
		{
			try
			{
				TypeDefinition destinationType = destinationModule.Types.Single (t => t.Name == destinationClassName);
				MethodDefinition destinationMethod = destinationType.Methods.Single(m => m.Name == destinationMethodName);
				ILProcessor destinationMethodProcessor = destinationMethod.Body.GetILProcessor();

				List<Instruction> instructionsToWeave = new List<Instruction>();

				foreach (object arg in customFieldConstructorArgs) {
					// VERY BROKEN
					Instruction loadArg = destinationMethodProcessor.Create(OpCodes.Ldc_I4, 0);
					instructionsToWeave.Add(loadArg);
				}

				Instruction loadNewObjectConstructorInstruction =
					destinationMethodProcessor.Create(OpCodes.Newobj,
						destinationMethod.Module.Import(
							customFieldType.GetConstructor(customFieldConstructorArgTypes)));
				instructionsToWeave.Add(loadNewObjectConstructorInstruction);

				OpCode storageOpcode;
				if (customFieldAttributes.HasFlag(Mono.Cecil.FieldAttributes.Static))
				{
					storageOpcode = OpCodes.Stsfld;
				} 
				else
				{
					storageOpcode = OpCodes.Stfld;		
				}

				Instruction storeInitializedObjectIntoField = 
					destinationMethodProcessor.Create(storageOpcode,
						destinationType.Fields.Single(f => f.Name == customFieldName));
				instructionsToWeave.Add(storeInitializedObjectIntoField);

				Weaving.WeaveListOfInstructionsAtMethodEntry(
					methodToWeave: destinationMethod,
					listOfInstructionsToWeave: instructionsToWeave
				);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTest custom field constructor handler caught exception " + e.Message);
			}
		}

		public static void InvokeMethodOfPublicCustomField(
			ModuleDefinition destinationModule,
			string destinationTypeName, 
			string destinationMethodName,

			string customFieldTypeName,
			string customFieldName,
			string customFieldMethodToInvoke,
			Type customFieldType,

			// TODO maybe create a class to encapsulate this more cleanly
			Type[] invokedMethodArgTypes, 
			object[] invokedMethodArgs,
			bool weavePositionIsStart
		)
		{
			try
			{
				TypeDefinition destinationType = destinationModule.Types.Single(t => t.Name == destinationTypeName);
				MethodDefinition destinationMethod = destinationType.Methods.Single(m => m.Name == destinationMethodName);
				ILProcessor destinationMethodProcessor = destinationMethod.Body.GetILProcessor();

				TypeDefinition customFieldParentType = destinationModule.Types.Single(t => t.Name == customFieldTypeName);
				FieldDefinition customField = customFieldParentType.Fields.Single(fn => fn.Name == customFieldName);

				List<Instruction> listOfInstructions = new List<Instruction>();

				//Console.WriteLine("debug 1");

				// Load the static object onto the stack
				OpCode customFieldLoadOpCode;
				if (customField.IsStatic)
				{
					customFieldLoadOpCode = OpCodes.Ldsfld;
				}
				else
				{
					customFieldLoadOpCode = OpCodes.Ldfld;
				}
				Instruction loadCustomFieldObject =
					destinationMethodProcessor.Create(customFieldLoadOpCode, customField);
				listOfInstructions.Add(loadCustomFieldObject);

				//Console.WriteLine("debug 2");

				foreach (object arg in invokedMethodArgs) {
					// Issues here again
					Instruction loadArg = destinationMethodProcessor.Create(OpCodes.Ldstr, arg.ToString());
					listOfInstructions.Add(loadArg);
				}

				//Console.WriteLine("debug 3");

				// Call the method in question 
				// TODO generalize eventually, ok atm
				Instruction invokeMethodOnCustomFieldInstance =
					destinationMethodProcessor.Create(OpCodes.Callvirt,
						destinationModule.Import(
							customFieldType.GetMethod(customFieldMethodToInvoke, invokedMethodArgTypes)));
				listOfInstructions.Add(invokeMethodOnCustomFieldInstance);

				//Console.WriteLine("debug 4");

				if (weavePositionIsStart)
				{
					Weaving.WeaveListOfInstructionsAtMethodEntry(
						methodToWeave: destinationMethod,
						listOfInstructionsToWeave: listOfInstructions
					);
				}
				else
				{
					Weaving.WeaveListOfInstructionsAtMethodExit(
						methodToWeave: destinationMethod,
						listOfInstructionsToWeave: listOfInstructions
					);
				}

				//Console.WriteLine("debug 5");
			}

			catch (Exception e)
			{
				Console.WriteLine("FlowTest custom field invoke handler caught exception " + e.Message + " " + e.StackTrace);
			}

			// Basically, "this"
			// Instruction loadSelfReference = destinationMethodProcessor.Create(OpCodes.Ldarg_0);
			// listOfInstructions.Add(loadSelfReference);
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

			Instruction[] arrayOfInstructionsToWeave = instructionsToWeave.ToArray();
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

		public static string GetStandaloneFTFieldName(
			string TypeName
		)
		{
			return "_FT_" + TypeName;
		}

		public static void ConsoleWriteEachCILInstruction(
			MethodDefinition methodToPrint
		)
		{
			Console.WriteLine ("v---- Instructions in {0} ----v", methodToPrint.FullName);
			ILProcessor instructionProcessor = methodToPrint.Body.GetILProcessor();

			foreach(Instruction ii in instructionProcessor.Body.Instructions) {
				Console.WriteLine(ii);
			}
			Console.WriteLine ("^---- End Instructions in {0} ----^", methodToPrint.FullName);
		}

		#endregion
	}
}

