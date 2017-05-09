using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FlowTestAPI
{
	public class FlowTestWeaver
	{
		private ModuleDefinition mModule;

		private string moduleReadPath;
		private string moduleWritePath;

		public FlowTestWeaver(string sourceModulePath, string destinationModulePath)
		{
			moduleReadPath = sourceModulePath;
			moduleWritePath = destinationModulePath;
		
			mModule = ModuleDefinition.ReadModule(moduleReadPath);
		}

		public void WriteInstrumentedCodeToFile()
		{
			mModule.Write (moduleWritePath);
		}

		// Points of Interest
		public void WeaveWatchpointAtPointOfInterest(FlowTestPointOfInterest point)
		{
			WeavingAtLocation.WeaveModuleAtTargetPointCall (mModule, point);
		}
	}
}


		/*public static void BindModuleToTestDriver(
			string modulePath, 
			string destinationPath)
		{
			try
			{
				//
				// The first bytecode modification involves adding a public static field into the
				// main class of the module - the equivalent of finding the entry point and plopping it
				// right at the top. We won't initialize the field, just create it here and give it a name.
				// Though the target application references the FlowTestAPI assembly, we have to make sure
				// it knows about the FlowTestAwayTeam, so we import that.
				//
				TypeDefinition moduleMainClassType = module.Types.Single(t => t.Name == "MainClass");

				string testDriverHookFieldName = "mTestDriverHook";
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
				TypeDefinition echoServerKitchenSinkType = module.Types.Single(t => t.Name == "ChatServer");
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
			}
		}*/
