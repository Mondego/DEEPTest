using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace FlowTestAPI
{
	public class FlowTestWeaver
	{
		public static void BindModuleToTestDriver(
			string modulePath, 
			string destinationPath)
		{
			try
			{
				// Some boilerplate code
				string readModuleFromPath = modulePath;
				string writeModuleToPath = destinationPath;

				ModuleDefinition module = ModuleDefinition.ReadModule(readModuleFromPath);

				//
				// The first bytecode modification involves adding a public static field into the
				// main class of the module - the equivalent of finding the entry point and plopping it
				// right at the top. We won't initialize the field, just create it here and give it a name.
				// Though the target application references the FlowTestAPI assembly, we have to make sure
				// it knows about the FlowTestAwayTeam, so we import that.
				//
				TypeDefinition moduleMainClassType = module.Types.Single(t => t.Name == "MainClass");

				string testDriverHookFieldName = "mTestDriverHook";
				/*FieldDefinition addTestDriverHookFieldDef = new FieldDefinition(
					testDriverHookFieldName,
					Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
					module.Import(typeof(FlowTestAwayTeam)));
				moduleMainClassType.Fields.Add(addTestDriverHookFieldDef);*/
				WeavingCustomFields.WeavePublicStaticFieldIntoModuleEntry(
					m: module,
					customFieldName: "mTestDriverHook",
					fieldType: typeof(FlowTestAwayTeam)
				);

				//
				// Next we need to invoke a constructor for a FlowTestAwayTeam named mTestDriverHook, 
				// which lives in the main class of an object we are going to fetch values from. 
				// 
				// To do this, we need to 
				//     (1) Grab pointers to the method we're interested in ("Main") and its instruction processor.
				//     (2) Load the test driver and test driver hook ports onto the stack (60011, 60012 respectively)
				//     (3) Invoke the constructor 
				MethodDefinition moduleEntryMethod = moduleMainClassType.Methods.Single(m => m.Name == "Main");
				ILProcessor moduleEntryMethodProcessor = moduleEntryMethod.Body.GetILProcessor();

				Instruction loadTestDriverPortNumber = moduleEntryMethodProcessor.Create(OpCodes.Ldc_I4, 60011);
				Instruction loadTestDriverHookPortNumber = moduleEntryMethodProcessor.Create(OpCodes.Ldc_I4, 60012);

				Instruction createTestDriverHookObjectOnStack = 
					moduleEntryMethodProcessor.Create(OpCodes.Newobj, 
						moduleEntryMethod.Module.Import(
							typeof(FlowTestAwayTeam).GetConstructor(new [] { typeof(int), typeof(int) })));

				Instruction storeTestDriverHookObject =
					moduleEntryMethodProcessor.Create(OpCodes.Stsfld,
						moduleMainClassType.Fields.Single(f => f.Name == testDriverHookFieldName));

				moduleEntryMethodProcessor.InsertBefore(moduleEntryMethod.Body.Instructions.First(), loadTestDriverPortNumber);
				moduleEntryMethodProcessor.InsertAfter(loadTestDriverPortNumber, loadTestDriverHookPortNumber);
				moduleEntryMethodProcessor.InsertAfter(loadTestDriverHookPortNumber, createTestDriverHookObjectOnStack);
				moduleEntryMethodProcessor.InsertAfter(createTestDriverHookObjectOnStack, storeTestDriverHookObject);

				//
				// In the next part, we insert a registration statement into the constructor of our server.
				// First, we gather the basic utilities: TypeDefinition, then MethodDefinition, then an ILProcessor.
				// Then, create instructions for 
				//    (1) loading the static field from the module main class onto the stack,
				//    (2) loading a pointer to self onto the stack
				//    (3) and finally a virtual register call.
				//
				TypeDefinition echoServerKitchenSinkType = module.Types.Single(t => t.Name == "EchoServer");
				MethodDefinition echoServerConstructor = echoServerKitchenSinkType.Methods.Single(m => m.Name == ".ctor");
				ILProcessor constructorInstructionProcessor = echoServerConstructor.Body.GetILProcessor();

				Instruction loadStaticTestDriverHook = 
					constructorInstructionProcessor.Create(OpCodes.Ldsfld, 
						moduleMainClassType.Fields.Single(f => f.Name == testDriverHookFieldName));  
				Instruction loadSelfReference = constructorInstructionProcessor.Create(OpCodes.Ldarg_0);
				Instruction callRegistrationInstruction = 
					constructorInstructionProcessor.Create(OpCodes.Callvirt, 
						echoServerConstructor.Module.Import(
							typeof (FlowTestAwayTeam).GetMethod ("EntangleWithLocalTestRuntime", new [] { typeof (object) })));

				constructorInstructionProcessor.InsertAfter(
					echoServerConstructor.Body.Instructions.First(), 
					loadStaticTestDriverHook);
				constructorInstructionProcessor.InsertAfter(
					loadStaticTestDriverHook,
					loadSelfReference);
				constructorInstructionProcessor.InsertAfter(
					loadSelfReference,
					callRegistrationInstruction);

				module.Write(writeModuleToPath);
			}

			catch (Exception e) {
				Console.WriteLine ("Weaver captured exception - " + e.Message);
			}
		}

		public static void PrintInstructionsInTargetMethod(
			string executablePath, 
			string targetType, 
			string targetMethod)
		{
			string readModuleFromPath = executablePath;
			ModuleDefinition module = ModuleDefinition.ReadModule(readModuleFromPath);

			var weaveTargetType = module.Types.Single(t => t.Name == targetType);
			var weaveTargetMethod = weaveTargetType.Methods.Single(m => m.Name == targetMethod);
			var weavingProcessor = weaveTargetMethod.Body.GetILProcessor();
			foreach (Instruction i in weavingProcessor.Body.Instructions) {
				Console.WriteLine(i);
			}
		}

		private static void InsertLabeledDebugStatement(
			MethodDefinition targetMethod,
			Instruction insertAfterInstruction,
			string label,
			object printDebug
		)
		{
			ILProcessor instructionProcessor = targetMethod.Body.GetILProcessor();

			Instruction loadLabelInstruction = instructionProcessor.Create(OpCodes.Ldstr, "[debug] " + label + ": ");

			Instruction loadDebugStringInstruction = instructionProcessor.Create(OpCodes.Ldstr, printDebug.ToString());

			Instruction concatenateStringsInstruction = 
				instructionProcessor.Create(OpCodes.Call, 
					targetMethod.Module.Import(
						typeof (String).GetMethod ("Concat", new [] { typeof (string), typeof (string) })));

			Instruction writeValueToConsoleInstruction = 
				instructionProcessor.Create(OpCodes.Call, 
					targetMethod.Module.Import(
						typeof (Console).GetMethod ("WriteLine", new [] { typeof (string) })));

			instructionProcessor.InsertAfter(insertAfterInstruction, loadLabelInstruction);
			instructionProcessor.InsertAfter(loadLabelInstruction, loadDebugStringInstruction);
			instructionProcessor.InsertAfter(loadDebugStringInstruction, concatenateStringsInstruction);
			instructionProcessor.InsertAfter(concatenateStringsInstruction, writeValueToConsoleInstruction);
		}
	}
}

