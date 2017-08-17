using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace FlowTest
{
	public class Weaving
	{
		public static void WeavePointofInterest(
			ModuleDefinition module, 
			FlowTestPointOfInterest poi
		)
		{
			try {
				TypeDefinition destinationType = module.Types.Single(t => t.Name == poi.parentTypeOfWatchpoint);
				MethodDefinition poiMethod = destinationType.Methods.Single(m => m.Name == poi.methodOfInterest);
				ILProcessor instructionProcessor = poiMethod.Body.GetILProcessor();

				if (poi.watchBefore)
				{
					WeavingFlowTestProxy.InvokeResultAggregatorBeforeMethod(
						method: poiMethod,
						key: poi.GetHashCode().ToString(),
						value: poi.generatePayload(content: "before")
					);

					WeavingDebug.WeaveDebugStatementBeforeMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened before " + poi.methodOfInterest
					);
				}

				if (poi.watchAfter)
				{
					WeavingFlowTestProxy.InvokeResultAggregatorAfterMethod(
						method: poiMethod,
						key: poi.GetHashCode().ToString(),
						value: poi.generatePayload(content: "after")
					);
						
					WeavingDebug.WeaveDebugStatementAfterMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened after " + poi.methodOfInterest
					);
				}
			}

			catch (Exception e) {
				Console.WriteLine ("| FlowTest Weaver caught an exception while adding a point of interest.");
				Console.WriteLine ("| PoI: {0} => {1}", poi.parentTypeOfWatchpoint, poi.methodOfInterest);
				Console.WriteLine ("| {0} {1}", e.InnerException, e.Message);
			}
		}
	}
}

