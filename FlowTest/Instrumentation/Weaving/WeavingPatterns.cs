using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace FlowTest
{
	public class WeavingPatterns
	{
		public static TypeDefinition WeaveSingletonPattern (
			ModuleDefinition module,
			string typeName
		)
		{
			#if DEBUG
			Console.WriteLine("Adding Singleton type named {0} into Module {1}",
				typeName, module.Name);
			#endif

			/**
			* public sealed class [typeName]
			* {
			* private static readonly [typename] instance = new [typename]()
			* 
			* // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			* static [typeName] () {}
			* private [typeName] () {}
			* 
			* public static [typeName] Instance { get { return instance; } }
			*/

			// public sealed class [typeName]
			TypeDefinition singletonType = WeavingBuildingBlocks._AddTypeDefinitionToModule(
				moduleDefinition: module,
				nameOfType: typeName,
				destinationNamespace: "",
				typeAttributes: Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Sealed
			);
			TypeReference singletonTypeReference = new TypeReference(
				@namespace: "",
				name: typeName,
				module: module,
				scope: module
			);

			// private static readonly [typename] instance = new [typename]()
			// the initialization will happen in the static constructor 
			FieldDefinition instanceFieldDefinition = WeavingBuildingBlocks._AddFieldDefinitionToType(
				moduleDefinition: module,
				typeDefinition: singletonType,
				nameOfField: "instance",
				attributesOfField: Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.InitOnly,
				typeReferenceOfField: singletonTypeReference
			);

			// Constructors
			// static [typeName] () {}
			// private [typeName] () {}
			// TODO see if needs MethodAttributes.CompilerControlled
			MethodDefinition staticConstructor = WeavingBuildingBlocks._AddMethodDefinitionToType (
				moduleDefinition: module,
				typeDefinition: singletonType,
				nameOfMethod: ".cctor",
				methodAttributes: MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: module.Import(typeof(void)),
				methodParameters: new List<ParameterDefinition>()
			);

			MethodDefinition privateConstructor = WeavingBuildingBlocks._AddMethodDefinitionToType (
				moduleDefinition: module,
				typeDefinition: singletonType,
				nameOfMethod: ".ctor",
				methodAttributes: MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: module.Import (typeof(void)),
				methodParameters: new List<ParameterDefinition> ()
			);

			// Instance property
			// public static [typeName] Instance { get { return instance; } }
			PropertyDefinition instanceProperty = WeavingBuildingBlocks._AddPropertyDefinitionToType(
				moduleDef: module,
				typeDef: singletonType,
				nameOfProperty: "Instance",
				propertyAttributes: Mono.Cecil.PropertyAttributes.None,
				propertyTypeReference: singletonTypeReference
			);
			MethodDefinition getInstancePropertyMethod = 
				WeavingBuildingBlocks.AddPropertyGetter (instanceProperty);

			// Filling in methods

			// CCTOR
			MethodReference ctorRef = new MethodReference (                         
				name: ".ctor",                           
				returnType: module.Import(typeof(void)),
				declaringType: singletonTypeReference
			);
			FieldReference instanceRef = new FieldReference (                             
				name: "instance",
				fieldType: singletonTypeReference,
				declaringType: singletonTypeReference
			);
			staticConstructor.Body.SimplifyMacros ();
			staticConstructor.Body.Instructions.Add (
				staticConstructor.Body.GetILProcessor().Create (
					OpCodes.Newobj, ctorRef));
			staticConstructor.Body.Instructions.Add(
				staticConstructor.Body.GetILProcessor().Create(OpCodes.Stsfld, instanceRef));
			staticConstructor.Body.Instructions.Add(
				staticConstructor.Body.GetILProcessor().Create(OpCodes.Ret));
			staticConstructor.Body.OptimizeMacros ();

			// CTOR
			MethodReference systemBaseCtorReference = new MethodReference (                                
				name: ".ctor",                                    
				returnType: module.Import(typeof(void)),
				declaringType: module.Import(typeof(Object))
			);
			privateConstructor.Body.SimplifyMacros ();
			privateConstructor.Body.Instructions.Add(
				privateConstructor.Body.GetILProcessor().Create(
					OpCodes.Ldarg_0));
			privateConstructor.Body.Instructions.Add(
				privateConstructor.Body.GetILProcessor().Create(
					OpCodes.Call, systemBaseCtorReference));
			privateConstructor.Body.Instructions.Add(
				privateConstructor.Body.GetILProcessor().Create(OpCodes.Ret));
			privateConstructor.Body.OptimizeMacros ();

			// GET_INSTANCE
			getInstancePropertyMethod.Body.SimplifyMacros ();
			getInstancePropertyMethod.Body.Instructions.Add (
				getInstancePropertyMethod.Body.GetILProcessor ().Create (
					OpCodes.Ldsfld, instanceRef));
			// IL_0006: stloc.0
			// IL_0007: br IL_000c
			getInstancePropertyMethod.Body.Instructions.Add(
				getInstancePropertyMethod.Body.GetILProcessor().Create(
					OpCodes.Ret));
			getInstancePropertyMethod.Body.OptimizeMacros ();

			return singletonType;
		}

	}
}

