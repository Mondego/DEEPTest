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
			TypeDefinition simpler = 
				WeavingPatterns.WeaveThreadSafeStaticType(                     
					module: mModule,
				    typeName: "FlowTestProxy"
			    );

			/*TypeDefinition FlowTestSingleton = WeavingPatterns.WeaveSingletonPattern (
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
			resultAggregator.Body.OptimizeMacros ();*/
		}

		public static void InvokeResultAggregatorBeforeMethod(
			MethodDefinition method,
			string value
		)
		{

			/*List<Instruction> proxyInvocationInstructions = new List<Instruction>();
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
			);*/
		}

		public static void InvokeResultAggregatorAfterMethod(
			MethodDefinition method,
			string value
		)
		{
			List<Instruction> proxyInvocationInstructions = new List<Instruction>();

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Call,
					method.Module.Import(
						new MethodReference(
							name: "DoMessage",
							returnType: method.Module.Import(typeof(void)),
							declaringType: new TypeReference(
								@namespace: "",
								name: "FlowTestProxy",
								module: method.Module,
								scope: method.Module
							)
						)
					)
				)
			);

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodExit (
				methodToWeave: method,
				listOfInstructionsToWeave: proxyInvocationInstructions
			);
				
			/*
			TypeDefinition testSingleton = method.Module.Types.Single(tt => tt.Name == "TestSingleton");
			TypeDefinition proxySingleton = method.Module.Types.Single(t => t.Name == "FlowTestProxySingleton");

			Console.WriteLine("+++++ FIELDS +++++");
			foreach (FieldDefinition fd in testSingleton.Fields) {
				Console.WriteLine(fd.FullName + " " + fd.Attributes);
			}
			foreach (FieldDefinition fd in proxySingleton.Fields) {
				Console.WriteLine(fd.FullName + " " + fd.Attributes);
			}

			Console.WriteLine("+++++ PROPERTIES +++++");
			foreach (PropertyDefinition pd in testSingleton.Properties) {
				Console.WriteLine(pd.FullName + " " + pd.Attributes);
			}
			foreach (PropertyDefinition pd in proxySingleton.Properties) {
				Console.WriteLine(pd.FullName + " " + pd.Attributes);
			}

			Console.WriteLine("+++++ METHODS +++++");
			foreach (MethodDefinition md in testSingleton.Methods) {
				Console.WriteLine(md.FullName + " " + md.Attributes);
				foreach (Instruction i in md.Body.Instructions) {
					Console.WriteLine(i);
				}
			}
			foreach (MethodDefinition md in proxySingleton.Methods) {
				Console.WriteLine(md.FullName + " " + md.Attributes);
				foreach (Instruction i in md.Body.Instructions) {
					Console.WriteLine(i);
				}
			}
			Console.WriteLine("++++");

			MethodDefinition sendResultMethod = proxySingleton.Methods.Single(m => m.Name == "SendResult");
			MethodDefinition getInstanceMethod = proxySingleton.Methods.Single(m => m.Name == "get_Instance");

			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Call,
					(MethodReference)getInstanceMethod));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, aggregation));
			proxyInvocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Callvirt,
					(MethodReference)sendResultMethod));
				
		*/
		}
	}
}

