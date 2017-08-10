using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;

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
				destinationNamespace: "SampleServer",
				typeAttributes: Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Sealed
			);
			TypeReference singletonTypeReference = new TypeReference(
				@namespace: "SampleServer",
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

			Instruction loadInstanceFld = 
				getInstancePropertyMethod.Body.GetILProcessor().Create(  
					OpCodes.Ldsfld, instanceRef);
			Instruction storeInstanceFld = 
				getInstancePropertyMethod.Body.GetILProcessor().Create(
				    OpCodes.Stloc_0);
			Instruction loadInstance = 
				getInstancePropertyMethod.Body.GetILProcessor().Create(                         
					OpCodes.Ldloc_0);
			Instruction branch =
				getInstancePropertyMethod.Body.GetILProcessor().Create(
					OpCodes.Br, loadInstance);
			Instruction ret = 
				getInstancePropertyMethod.Body.GetILProcessor().Create(
				    OpCodes.Ret);
			Instruction nop =
				getInstancePropertyMethod.Body.GetILProcessor().Create(
					OpCodes.Nop);
			
			getInstancePropertyMethod.Body.Instructions.Add(loadInstance);
			getInstancePropertyMethod.Body.GetILProcessor().InsertAfter(loadInstance, ret);
			getInstancePropertyMethod.Body.GetILProcessor().InsertBefore(loadInstance, branch);
			getInstancePropertyMethod.Body.GetILProcessor().InsertBefore(branch, storeInstanceFld);
			getInstancePropertyMethod.Body.GetILProcessor().InsertBefore(storeInstanceFld, loadInstanceFld);
			getInstancePropertyMethod.Body.GetILProcessor().InsertBefore(loadInstanceFld, nop);
			getInstancePropertyMethod.Body.OptimizeMacros ();

			return singletonType;
		}

		public static TypeDefinition WeaveThreadSafeStaticType (
			ModuleDefinition module,
			string typeName
		)
		{
			// FlowTestProxy type
			TypeDefinition flowTestProxyType = WeavingBuildingBlocks._AddTypeDefinitionToModule(
				moduleDefinition: module,
				nameOfType: "FlowTestProxy",
				destinationNamespace: "",
				typeAttributes: TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed
			);
				
			// Fields
			FieldDefinition lockingField = WeavingBuildingBlocks._AddFieldDefinitionToType(
				moduleDefinition: module,
				typeDefinition: flowTestProxyType,
				nameOfField: "locking",
				attributesOfField: FieldAttributes.Private | FieldAttributes.Static,
				typeReferenceOfField: module.Import(typeof(System.Object))
			);

			// Methods
			MethodDefinition cctor = WeavingBuildingBlocks._AddMethodDefinitionToType (
				moduleDefinition: module,
				typeDefinition: flowTestProxyType,
				nameOfMethod: ".cctor",
				methodAttributes: MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: module.Import(typeof(void)),
				methodParameters: new List<ParameterDefinition>()
			);
			cctor.Body.SimplifyMacros();
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Newobj,
					module.Import(typeof(System.Object).GetConstructor(new Type[] { }))));
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Stsfld,
					(FieldReference)lockingField));
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(OpCodes.Ret));
			cctor.Body.OptimizeMacros();

			MethodDefinition onLockMethod = 
				WeavingBuildingBlocks._AddMethodDefinitionToType(
					moduleDefinition: module,
					typeDefinition: flowTestProxyType,
					nameOfMethod: "onLock",
					methodAttributes: MethodAttributes.Public | MethodAttributes.Static,
					returnTypeReferenceOfMethod: module.Import(typeof(void)),
					methodParameters: new List<ParameterDefinition>()    
				);
			onLockMethod.Body.SimplifyMacros();
			onLockMethod.Body.Instructions.Add(
				onLockMethod.Body.GetILProcessor().Create(
					OpCodes.Ret));
			onLockMethod.Body.OptimizeMacros();

			MethodDefinition DoMessage = 
				WeavingBuildingBlocks._AddMethodDefinitionToType(
					moduleDefinition: module,
					typeDefinition: flowTestProxyType,
					nameOfMethod: "DoMessage",
					methodAttributes: MethodAttributes.Public | MethodAttributes.Static,
					returnTypeReferenceOfMethod: module.Import(typeof(void)),
					methodParameters: new List<ParameterDefinition>()
				);

			// public static void DoMessage(string msg)
			// {
			//   lock (theLock)
			//   {
			// 	   onLock();
			//   }
		    // }
			ILProcessor dmil = DoMessage.Body.GetILProcessor();
			DoMessage.Body.SimplifyMacros();
			DoMessage.Body.Instructions.Add(dmil.Create(
				OpCodes.Ldstr, "pilot"));
			DoMessage.Body.Instructions.Add(dmil.Create(
				OpCodes.Call,
				module.Import(typeof(Console).GetMethod("WriteLine", new [] { typeof(string) }))));
				
			/*
			+IL_0001: ldsfld System.Object SampleServer.TestThreadsafe::theLock
			+IL_0006: stloc.0
			+IL_0007: ldc.i4.0
			+IL_0008: stloc.1
			+IL_0009: ldloc.0
			+IL_000a: ldloca.s V_1
			+IL_000c: call System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)
			+IL_0011: nop
			+IL_0012: call System.Void SampleServer.TestThreadsafe::onLock()
			+IL_0017: nop
			+IL_0018: leave IL_0027
			+IL_001d: ldloc.1
			+IL_001e: brfalse.s IL_0026
			+IL_0020: ldloc.0
			+IL_0021: call System.Void System.Threading.Monitor::Exit(System.Object)
			+IL_0026: endfinally
			+IL_0027: ret*/
			DoMessage.Body.Instructions.Add(dmil.Create(OpCodes.Ret));
			DoMessage.Body.OptimizeMacros();

			///////////////
			//foreach (MethodDefinition mi in module.Types.Single(t => t.Name == "TestThreadsafe").Methods) {
			// foreach (MethodDefinition mi in flowTestProxyType.Methods) {

			return flowTestProxyType;
		}
	}
}
