using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Collections.Generic;
using System.Linq;

namespace FlowTest
{
	public class WeavingFlowTestProxy
	{
		public static void WeaveSingletonForFlowTestProxy(
			ModuleDefinition mModule
		)
		{
			TypeDefinition FlowTestSingleton = WeavingPatterns.WeaveSingletonPattern (
				module: mModule,
				typeName: "FlowTestProxySingleton"
			);

			MethodDefinition resultAggregator = WeavingBuildingBlocks._AddMethodDefinitionToType (        
				moduleDefinition: mModule,
				typeDefinition: FlowTestSingleton,	                                    
				nameOfMethod: "SendResult",      
				methodAttributes: MethodAttributes.Public,             
				returnTypeReferenceOfMethod: mModule.Import (typeof(void)),
				methodParameters: WeavingBuildingBlocks.ParameterBuilder(
					moduleDef: mModule,                                    
					parameterNames: new string[] { "resultString" },                          
					parameterTypes: new Type[] { typeof(string) }              
				)                                   
			);

			resultAggregator.Body.SimplifyMacros ();
			resultAggregator.Body.Instructions.Add(
				resultAggregator.Body.GetILProcessor().Create(
					OpCodes.Ldarg_1));
			resultAggregator.Body.Instructions.Add(
				resultAggregator.Body.GetILProcessor().Create(
					OpCodes.Call,
					resultAggregator.Module.Import(
						typeof(Console).GetMethod("WriteLine", new [] { typeof(string) }))));
			resultAggregator.Body.Instructions.Add (
				resultAggregator.Body.GetILProcessor ().Create (
					OpCodes.Ret));
			resultAggregator.Body.OptimizeMacros ();
		}

		public static TypeReference getFlowTestProxyTypeReference(
			ModuleDefinition m
		)
		{
			return new TypeReference(
				@namespace: "",
				name: "FlowTestProxySingleton",
				module: m,
				scope: m
			);
		}

		public static MethodReference getAggregationMethodReference(
			ModuleDefinition m
		)
		{
			return new MethodReference(
				name: "SendResult",
				returnType: m.Import(typeof(void)),
				declaringType: getFlowTestProxyTypeReference(m)
			);
		}

		public static void InvokeResultAggregatorBeforeMethod(
			MethodDefinition method,
			string value
		)
		{
			List<Instruction> proxyInvocationInstructions = new List<Instruction>();
			string aggregation = "[proxy before] " + value;

			TypeReference tr = new TypeReference(
				@namespace: "",
				name: "FlowTestProxySingleton",
				module: method.Module,
				scope: method.Module
			);

			MethodReference mm = new MethodReference(
				name: "SendResult",
				returnType: method.Module.Import(typeof(void)),
				declaringType: tr
			);
			method.Module.Import(mm).Resolve();

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, aggregation));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Callvirt, mm.Resolve()));
			
			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodEntry (
				methodToWeave: method,
				listOfInstructionsToWeave: proxyInvocationInstructions
			);
		}

		public static void InvokeResultAggregatorAfterMethod(
			MethodDefinition method,
			string value
		)
		{
			List<Instruction> proxyInvocationInstructions = new List<Instruction>();
			string aggregation = "[proxy after] " + value;

			TypeReference tr = new TypeReference(
				@namespace: "",
				name: "FlowTestProxySingleton",
				module: method.Module,
				scope: method.Module
			);

			MethodReference mm = new MethodReference(
				name: "SendResult",
				returnType: method.Module.Import(typeof(void)),
				declaringType: tr
			);
			method.Module.Import(mm).Resolve();

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, aggregation));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Callvirt, mm.Resolve()));
				
			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodExit (
				methodToWeave: method,
				listOfInstructionsToWeave: proxyInvocationInstructions
			);
		}
	}
}

