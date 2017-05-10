using System;
using Mono.Cecil;
using System.Linq;

namespace FlowTestAPI
{
	public class WeavingCustomFields
	{
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

		public static void InitializeField () {
			/*MethodDefinition moduleEntryMethod = moduleMainClassType.Methods.Single(m => m.Name == "Main");
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
			moduleEntryMethodProcessor.InsertAfter(createTestDriverHookObjectOnStack, storeTestDriverHookObject);*/

		}

		public static void InvokeMethodOfCustomField()
		{
			/*
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
				callRegistrationInstruction);*/
		}
	}
}

