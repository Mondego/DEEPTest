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
			WeavingPatterns.WeaveThreadSafeStaticType(                     
				module: mModule,
				typeName: "FlowTestProxy"
			);

			/* WeavingPatterns.WeaveSingletonPattern (
				module: mModule,
				typeName: "FlowTestProxySingleton"
			);*/
		}

		public static void InvokeResultAggregatorBeforeMethod(
			MethodDefinition method,
			string value
		)
		{
			List<Instruction> proxyInvocationInstructions = new List<Instruction>();
			TypeDefinition proxyType = method.Module.Types.Single(t => t.Name == "FlowTestProxy");
			MethodDefinition invokeMessageMethod = proxyType.Methods.Single(m => m.Name == "DoMessage");

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, value));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Call,
					method.Module.Import(invokeMessageMethod)));

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodEntry(
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
			TypeDefinition proxyType = method.Module.Types.Single(t => t.Name == "FlowTestProxy");
			MethodDefinition invokeMessageMethod = proxyType.Methods.Single(m => m.Name == "DoMessage");

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, value));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Call,
					method.Module.Import(invokeMessageMethod)));

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodExit (
				methodToWeave: method,
				listOfInstructionsToWeave: proxyInvocationInstructions
			);
		}
	}
}

