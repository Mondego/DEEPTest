﻿using Mono.Cecil;
using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace FlowTest
{
	public class WeavingBuildingBlocks
	{
		#region Type Definitions

		public static TypeDefinition _AddTypeDefinitionToModule
		(
			ModuleDefinition moduleDefinition,
			string nameOfType,
			string destinationNamespace,
			TypeAttributes typeAttributes
		)
		{
			#if DEBUG
			Console.WriteLine("Adding TypeDefinition {0} into ModuleDefinition {1}", nameOfType, moduleDefinition.Name);
			#endif

			try
			{
				TypeDefinition addedTypeDefinition = new TypeDefinition (
					@namespace: destinationNamespace,
					name: nameOfType,
					attributes: typeAttributes,
					baseType: moduleDefinition.Import (typeof (object)));
			
				moduleDefinition.Types.Add (addedTypeDefinition);

				return addedTypeDefinition;
			}

			catch (Exception ex) {
				Console.WriteLine("Exception in WeavingBuildingBlocks._AddTypeDefinitionToModule {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		public static TypeDefinition TypePublicStaticWeaveHelper(
			ModuleDefinition module,
			string typeName
		)
		{
			return _AddTypeDefinitionToModule (
				moduleDefinition: module,
				nameOfType: typeName,
				destinationNamespace: "",
				typeAttributes: Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed
			);
		}
			
		#endregion

		#region Field Definitions

		public static FieldDefinition _AddFieldDefinitionToType
		(
			ModuleDefinition moduleDefinition,
			TypeDefinition typeDefinition,
			string nameOfField,
			FieldAttributes attributesOfField,
			TypeReference typeReferenceOfField
		)
		{
			#if DEBUG
			Console.WriteLine("Adding FieldDefinition {0} into TypeDefinition {1} in {2}", 
				nameOfField, typeDefinition.Name, moduleDefinition.Name);
			#endif

			try {
				FieldDefinition fieldDefinition = new FieldDefinition (
					name: nameOfField,
					attributes: attributesOfField,
					fieldType: typeReferenceOfField
				);

				typeDefinition.Fields.Add(fieldDefinition);

				return fieldDefinition;
			}

			catch (Exception ex) {
				Console.WriteLine("Exception in WeavingBuildingBlocks._AddFieldDefinitionToType {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		public static FieldDefinition FieldPublicStaticWeaveHelper(
			ModuleDefinition module,
			TypeDefinition typeContainingField,
			string fieldName,
			Type typeOfField
		)
		{
			return _AddFieldDefinitionToType (
				moduleDefinition: module,
				typeDefinition: typeContainingField,
				nameOfField: fieldName,
				attributesOfField: FieldAttributes.Public | FieldAttributes.Static,
				typeReferenceOfField: module.Import(typeOfField)
			);
		}

		#endregion

		#region Method Definitions

		public static MethodDefinition _AddMethodDefinitionToType
		(
			ModuleDefinition moduleDefinition,
			TypeDefinition typeDefinition,
			string nameOfMethod,
			MethodAttributes methodAttributes,
			TypeReference returnTypeReferenceOfMethod,
			List<ParameterDefinition> methodParameters
		)
		{
			#if DEBUG
			Console.WriteLine("Adding MethodDefinition {0} into TypeDefinition {1} in {2}", 
				nameOfMethod, typeDefinition.Name, moduleDefinition.Name);
			#endif

			try
			{
				MethodDefinition methodDefinition = new MethodDefinition (
					nameOfMethod,
					methodAttributes,
					returnTypeReferenceOfMethod
				);

				if (methodParameters != null && methodParameters.Count > 0)
				{
					foreach (ParameterDefinition param in methodParameters)
					{
						methodDefinition.Parameters.Add(param);
					}
				}
					
				typeDefinition.Methods.Add(methodDefinition);

				return methodDefinition;
			}

			catch (Exception ex) 
			{
				Console.WriteLine("Exception in WeavingBuildingBlocks._AddMethodDefinitionToType {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		public static MethodDefinition MethodStaticConstructorWeavingHelper
		(
			ModuleDefinition moduleDef,
			TypeDefinition typeDef,
			List<ParameterDefinition> parameters
		)
		{
			return _AddMethodDefinitionToType(
				moduleDefinition: moduleDef,
				typeDefinition: typeDef,
				nameOfMethod: ".cctor",
				methodAttributes: MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: moduleDef.Import(typeof(void)),
				methodParameters: parameters
			);
		}

		public static MethodDefinition MethodConstructorWeavingHelper
		(
			ModuleDefinition moduleDef,
			TypeDefinition typeDef,
			List<ParameterDefinition> parameters
		)
		{
			return _AddMethodDefinitionToType(
				moduleDefinition: moduleDef,
				typeDefinition: typeDef,
				nameOfMethod: ".ctor",
				methodAttributes: MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: moduleDef.Import(typeof(void)),
				methodParameters: parameters
			);
		}

		public static MethodDefinition MethodPublicStaticWeavingHelper
		(
			ModuleDefinition moduleDef,
			TypeDefinition typeDef,
			string @name,
			TypeReference returnType,
			List<ParameterDefinition> parameters
		)
		{
			return _AddMethodDefinitionToType(
				moduleDefinition: moduleDef,
				typeDefinition: typeDef,
				nameOfMethod: @name,
				methodAttributes: MethodAttributes.Static | MethodAttributes.Public,
				returnTypeReferenceOfMethod: returnType,
				methodParameters: parameters
			);
		}
			
		public static List<ParameterDefinition> ParameterBuilder(
			ModuleDefinition moduleDef,
			string[] parameterNames, 
			Type[] parameterTypes
		)
		{
			try
			{
				List<ParameterDefinition> parameters = new List<ParameterDefinition>();

				if (parameterNames.Length != parameterTypes.Length)
				{
					throw new System.ArgumentException(
						"ParameterBuilder array of parameter types should not have a different length than array of parameter names",
						"parameterTypes"
					);
				}

				for (int p = 0; p < parameterNames.Length; p++)
				{
					parameters.Add(new ParameterDefinition(
						name: parameterNames[p],
						attributes: ParameterAttributes.None,
						parameterType: moduleDef.Import(parameterTypes[p])
					));
				}

				return parameters;
			}

			catch (Exception ex) 
			{
				Console.WriteLine("Exception in WeavingBuildingBlocks.ParameterBuilder {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		#endregion

		#region PropertyDefinitions
		public static PropertyDefinition _AddPropertyDefinitionToType(
			ModuleDefinition moduleDef,
			TypeDefinition typeDef,
			string nameOfProperty,
			PropertyAttributes propertyAttributes,
			TypeReference propertyTypeReference
		)
		{
			#if DEBUG
			Console.WriteLine("Adding PropertyDefinition {0} into TypeDefinition {1} in {2}", 
				nameOfProperty, typeDef.Name, moduleDef.Name);
			#endif

			try
			{
				PropertyDefinition propertyDefinition = new PropertyDefinition (
					name: nameOfProperty,
					attributes: propertyAttributes,
					propertyType: propertyTypeReference
				);
		
				typeDef.Properties.Add(propertyDefinition);

				return propertyDefinition;
			}

			catch (Exception ex) 
			{
				Console.WriteLine("Exception in WeavingBuildingBlocks._AddPropertyDefinitionToType {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		public static MethodDefinition AddPropertyGetter (
			PropertyDefinition propertyDef
		)
		{
			return _AddMethodDefinitionToType(
				moduleDefinition: propertyDef.Module,
				typeDefinition: propertyDef.DeclaringType,
				nameOfMethod: "get_" + propertyDef.Name,
				methodAttributes: MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
				returnTypeReferenceOfMethod: propertyDef.PropertyType,
				methodParameters: new List<ParameterDefinition>()
			);
		}

		#endregion

		#region Weaving at Location

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
	}
}
