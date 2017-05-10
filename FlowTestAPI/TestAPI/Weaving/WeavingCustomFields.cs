using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;

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

				WeavingAtLocation._WeaveListOfInstructionsAtMethodEntry(
					methodToWeave: destinationMethod,
					listOfInstructionsToWeave: instructionsToWeave
				);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTest custom field constructor handler caught exception " + e.Message);
			}
		}

		public static void InvokeMethodOfCustomField(
			ModuleDefinition destinationModule,
			string destinationTypeName, 
			string destinationMethodName,

			string customFieldName,
			string invokingMethodName
		)
		{
			try
			{
				TypeDefinition destinationType = destinationModule.Types.Single (t => t.Name == destinationTypeName);
				MethodDefinition destinationMethod = destinationType.Methods.Single(m => m.Name == destinationMethodName);
				ILProcessor destinationMethodProcessor = destinationMethod.Body.GetILProcessor();

				Console.WriteLine("debugging weaving an invoke of {0}.{1}", customFieldName, invokingMethodName);
				Console.WriteLine("...");
				foreach (Instruction i in destinationMethodProcessor.Body.Instructions)
				{
					Console.WriteLine(i.Operand);
				}
				Console.WriteLine("...");
			}

			catch (Exception e)
			{
				Console.WriteLine("FlowTest custom field invoke handler caught exception " + e.Message);
			}

			/*Instruction loadStaticTestDriverHook = 
				constructorInstructionProcessor.Create(OpCodes.Ldsfld, 
					moduleMainClassType.Fields.Single(f => f.Name == testDriverHookFieldName));  
			Instruction loadSelfReference = constructorInstructionProcessor.Create(OpCodes.Ldarg_0);
			Instruction callRegistrationInstruction = 
				constructorInstructionProcessor.Create(OpCodes.Callvirt, 
					echoServerConstructor.Module.Import(
						typeof (FlowTestAwayTeam).GetMethod ("EntangleWithLocalTestRuntime", new [] { typeof (object) })));*/
		}
	}
}

