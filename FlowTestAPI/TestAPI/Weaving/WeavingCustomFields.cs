﻿using System;
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

				WeavingAtLocation.WeaveListOfInstructionsAtMethodEntry(
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

				// Basically, "this"
				// Instruction loadSelfReference = destinationMethodProcessor.Create(OpCodes.Ldarg_0);
				// listOfInstructions.Add(loadSelfReference);

				// Call the method in question 
				// TODO generalize eventually, ok atm
				Instruction invokeMethodOnCustomFieldInstance =
					destinationMethodProcessor.Create(OpCodes.Callvirt,
						destinationModule.Import(
							customFieldType.GetMethod(customFieldMethodToInvoke, new Type[] {})));
				listOfInstructions.Add(invokeMethodOnCustomFieldInstance);

				if (weavePositionIsStart)
				{
					WeavingAtLocation.WeaveListOfInstructionsAtMethodEntry(
						methodToWeave: destinationMethod,
						listOfInstructionsToWeave: listOfInstructions
					);
				}
				else
				{
					WeavingAtLocation.WeaveListOfInstructionsAtMethodExit(
						methodToWeave: destinationMethod,
						listOfInstructionsToWeave: listOfInstructions
					);
				}
			}

			catch (Exception e)
			{
				Console.WriteLine("FlowTest custom field invoke handler caught exception " + e.Message);
			}
		}
	}
}

