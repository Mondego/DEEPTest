using Mono.Cecil;
using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DeepTest
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
			TypeDefinition typeDefinition,
			string nameOfField,
			FieldAttributes attributesOfField,
			TypeReference typeReferenceOfField,
            object initialValue = null
        )
		{
			#if DEBUG
			Console.WriteLine("Adding FieldDefinition {0} into TypeDefinition {1} in {2}", 
				nameOfField, typeDefinition.Name, typeDefinition.Module.Name);
			#endif

			try {
				FieldDefinition fieldDefinition = new FieldDefinition (
					name: nameOfField,
					attributes: attributesOfField,
					fieldType: typeReferenceOfField
				);

                if (initialValue != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, initialValue);
                        fieldDefinition.InitialValue = ms.ToArray();
                    }
                }

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
			TypeDefinition typeContainingField,
			string fieldName,
			Type typeOfField,
            object initialVal = null
		)
		{
            return _AddFieldDefinitionToType(
                typeDefinition: typeContainingField,
                nameOfField: fieldName,
                attributesOfField: FieldAttributes.Public | FieldAttributes.Static,
                typeReferenceOfField: typeContainingField.Module.Import(typeOfField),
                initialValue: initialVal
            );
		}

		#endregion

		#region Method Definitions

		public static MethodDefinition _AddMethodDefinitionToType
		(
			TypeDefinition typeDefinition,
			string nameOfMethod,
			MethodAttributes methodAttributes,
			TypeReference returnTypeReferenceOfMethod
		)
		{
			#if DEBUG
			Console.WriteLine("Adding MethodDefinition {0} into TypeDefinition {1} in {2}", 
				nameOfMethod, typeDefinition.Name, typeDefinition.Module.Name);
			#endif

			try
			{
				MethodDefinition methodDefinition = new MethodDefinition (
					nameOfMethod,
					methodAttributes,
					returnTypeReferenceOfMethod
				);
					
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

		public static ParameterDefinition _AddParameterToMethodDefinition
		(
			MethodDefinition methodDef,
			string parameterName,
			Type paramType
		)
		{
			// TODO another version of this with a perameterattributes argument
			try
			{
				ParameterDefinition param = new ParameterDefinition(
					name: parameterName,
					attributes: ParameterAttributes.None,
					parameterType: methodDef.Module.Import(paramType)
				);

				methodDef.Parameters.Add(param);

				return param;
			}

			catch (Exception ex) 
			{
				Console.WriteLine("Exception in WeavingBuildingBlocks._AddParameterToMethodDefinition {0} {1}",
					ex.InnerException,
					ex.Message);

				return null;
			}
		}

		public static MethodDefinition CopyMethodSignature(
			MethodDefinition copyMethod,
			TypeDefinition pasteIntoType
		)
		{
			MethodDefinition paste = _AddMethodDefinitionToType(
				typeDefinition: pasteIntoType,
				nameOfMethod: copyMethod.Name,
				methodAttributes: copyMethod.Attributes,
				returnTypeReferenceOfMethod: pasteIntoType.Module.Import(copyMethod.ReturnType)
			);

			foreach (ParameterDefinition param in copyMethod.Parameters) {
				paste.Parameters.Add(new ParameterDefinition(
					name: param.Name,
					attributes: param.Attributes,
					parameterType: paste.Module.Import(param.ParameterType)
				));
			}

			return paste;
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
				typeDefinition: propertyDef.DeclaringType,
				nameOfMethod: "get_" + propertyDef.Name,
				methodAttributes: MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
				returnTypeReferenceOfMethod: propertyDef.PropertyType
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

		#region Instruction Helpers
	
		#endregion
	}
}

