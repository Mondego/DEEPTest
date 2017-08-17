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
		#region FlowTestProxy weaving
		public static TypeDefinition WeaveThreadSafeFlowTestProxyType (
			ModuleDefinition module,
			string typeName
		)
		{
			try {
				// FlowTestProxy type
				TypeDefinition flowTestProxyType = 
					WeavingBuildingBlocks._AddTypeDefinitionToModule(                                
						moduleDefinition: module,                                 
						nameOfType: "FlowTestProxy",             
						destinationNamespace: "",               
						typeAttributes: TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed   
					);

				// Fields
				FieldDefinition lockingField = 
					WeavingBuildingBlocks._AddFieldDefinitionToType( 
						typeDefinition: flowTestProxyType,    
						nameOfField: "locking",            
						attributesOfField: FieldAttributes.Private | FieldAttributes.Static,                           
						typeReferenceOfField: module.Import(typeof(System.Object))
					);

				FieldDefinition hostNameField = 
					WeavingBuildingBlocks._AddFieldDefinitionToType(
						typeDefinition: flowTestProxyType,
						nameOfField: "eventDestinationHostname",
						attributesOfField: FieldAttributes.Private | FieldAttributes.Static,
						typeReferenceOfField: module.Import(typeof(string))                            
					);

				FieldDefinition portField = 
					WeavingBuildingBlocks._AddFieldDefinitionToType(
						typeDefinition: flowTestProxyType,
						nameOfField: "eventDestinationPort",
						attributesOfField: FieldAttributes.Private | FieldAttributes.Static,
					    typeReferenceOfField: module.Import(typeof(int))
					);

				MethodDefinition cctor = proxyCctor(
					flowTestProxyType
				);
					
				MethodDefinition sendEvent = proxySendEvent(
					flowTestProxyType
				);
				////

				MethodDefinition onLock = proxyOnLock(
					flowTestProxyType
				);

				MethodDefinition requestLock = proxyRequestLock(
					flowTestProxyType
				);

				return flowTestProxyType;

			} catch (Exception bex) {
				Console.WriteLine(bex.Message + " " + bex.InnerException);
				return null;
			}
		}

		private static MethodDefinition proxyCctor(
			TypeDefinition td
		)
		{
			MethodDefinition cctor = WeavingBuildingBlocks._AddMethodDefinitionToType (
				typeDefinition: td,
				nameOfMethod: ".cctor",
				methodAttributes: MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
				returnTypeReferenceOfMethod: td.Module.Import(typeof(void))
			);
				
			cctor.Body.SimplifyMacros();
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Newobj,
					td.Module.Import(typeof(System.Object).GetConstructor(new Type[] { }))));
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Stsfld,
					td.Fields.Single(f => f.Name == "locking")));

			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Ldstr, "127.0.0.1"));
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Stsfld,
					td.Fields.Single(f => f.Name == "eventDestinationHostname")));

			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Ldc_I4, 60011));
			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(
					OpCodes.Stsfld,
					td.Fields.Single(f => f.Name == "eventDestinationPort")));

			cctor.Body.Instructions.Add(
				cctor.Body.GetILProcessor().Create(OpCodes.Ret));
			cctor.Body.OptimizeMacros();

			return cctor;
		}

		private static MethodDefinition proxySendEvent(
			TypeDefinition td
		)
		{
			MethodDefinition sendEventMethod = 
				WeavingBuildingBlocks._AddMethodDefinitionToType(
					typeDefinition: td,
					nameOfMethod: "SendEvent",
					methodAttributes: MethodAttributes.Public| MethodAttributes.Static,
					returnTypeReferenceOfMethod: td.Module.Import(typeof(void))
				);
			sendEventMethod.Parameters.Add(new ParameterDefinition(td.Module.Import(typeof(string))));
			sendEventMethod.Body.Variables.Add(new VariableDefinition(td.Module.Import(typeof(System.Net.Sockets.TcpClient))));
			sendEventMethod.Body.Variables.Add(new VariableDefinition(td.Module.Import(typeof(System.Net.Sockets.NetworkStream))));
			sendEventMethod.Body.Variables.Add(new VariableDefinition(td.Module.Import(typeof(System.Byte[]))));

			sendEventMethod.Body.SimplifyMacros();
			ILProcessor il = sendEventMethod.Body.GetILProcessor();

			//
			// 1. TcpClient tcpc = new TcpClient(eventDestinationHostname, eventDestinationPort);
			//
			// ldsfld System.String SampleServer.BootstrapMessenger::eventDestinationHostname
			// ldsfld System.Int32 SampleServer.BootstrapMessenger::eventDestinationPort
			// newobj System.Void System.Net.Sockets.TcpClient::.ctor(System.String,System.Int32)
			// stloc.0
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Ldsfld,
				td.Fields.Single(f => f.Name == "eventDestinationHostname")));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Ldsfld,
				td.Fields.Single(f => f.Name == "eventDestinationPort")));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Newobj,
				td.Module.Import(
					typeof(System.Net.Sockets.TcpClient)
					.GetConstructor(new [] { typeof(System.String), typeof(System.Int32) }))));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Stloc_0));
			//Console.WriteLine("Wrote instruction batch 1/6");

			//
			// 2. NetworkStream ns = tcpc.GetStream();
			//
			// ldloc.0
			// callvirt System.Net.Sockets.NetworkStream System.Net.Sockets.TcpClient::GetStream()
			// stloc.1
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_0));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Callvirt,
				td.Module.Import(
					typeof(System.Net.Sockets.TcpClient)
					.GetMethod("GetStream", new Type[] { }))));
			
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Stloc_1));
			//Console.WriteLine("Wrote instruction batch 2/6");

			//
			// 3. byte[] messageData = Encoding.ASCII.GetBytes(eventString);
			//
			// call System.Text.Encoding System.Text.Encoding::get_ASCII()
			// ldarg.0
			// callvirt System.Byte[] System.Text.Encoding::GetBytes(System.String)
			// stloc.2
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Call,
				td.Module.Import(
					typeof(System.Text.Encoding)
					.GetMethod("get_ASCII", new Type[] { }))));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldarg_0));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Callvirt,
				td.Module.Import(
					typeof(System.Text.Encoding).GetMethod("GetBytes", new [] { typeof(string) }))));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Stloc_2));
			//Console.WriteLine("Wrote instruction batch 3/6");

			//
			// 4. ns.Write(messageData, 0, messageData.Length);
			//
			// ldloc.1
			// ldloc.2
			// ldc.i4.0
			// ldloc.2
			// ldlen
			// conv.i4
			// callvirt System.Void System.IO.Stream::Write(System.Byte[],System.Int32,System.Int32)
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_1));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_2));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldc_I4_0));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_2));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldlen));
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Conv_I4));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Callvirt,
				td.Module.Import(
					typeof(System.IO.Stream)
					.GetMethod("Write", new [] { typeof(System.Byte[]), typeof(System.Int32), typeof(System.Int32) })
				)
			));
			//Console.WriteLine("Wrote instruction batch 4/6");

			//
			// 5. ns.Close();
			//
			// ldloc.1
			// callvirt System.Void System.IO.Stream::Close()
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_1));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Callvirt,
				td.Module.Import(
					typeof(System.IO.Stream)
					.GetMethod("Close", new Type[] { }))));
			//Console.WriteLine("Wrote instruction batch 5/6");

			//
			// 6. tcpc.Close();
			//
			// ldloc.0
			// callvirt System.Void System.Net.Sockets.TcpClient::Close()
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_0));
			sendEventMethod.Body.Instructions.Add(il.Create(
				OpCodes.Callvirt,
				td.Module.Import(
					typeof(System.Net.Sockets.TcpClient)
					.GetMethod("Close", new Type[] { }))));
			//Console.WriteLine("Wrote instruction batch 6/6");

			// ret
			sendEventMethod.Body.Instructions.Add(il.Create(OpCodes.Ret));
			sendEventMethod.Body.OptimizeMacros();

			return sendEventMethod;
		}

		private static MethodDefinition proxyOnLock(
			TypeDefinition td
		)
		{
			MethodDefinition onLockMethod = 
				WeavingBuildingBlocks._AddMethodDefinitionToType(
					typeDefinition: td,
					nameOfMethod: "OnLock",
					methodAttributes: MethodAttributes.Private | MethodAttributes.Static,
					returnTypeReferenceOfMethod: td.Module.Import(typeof(void))
				);
			onLockMethod.Parameters.Add(new ParameterDefinition(td.Module.Import(typeof(string))));
			onLockMethod.Parameters.Add(new ParameterDefinition(td.Module.Import(typeof(string))));

			onLockMethod.Body.SimplifyMacros();
			ILProcessor il = onLockMethod.Body.GetILProcessor();

			//
			// 1. SendEvent(key + "=" + value);
			// 
			// ldarg.1
			// call System.Void SampleServer.BootstrapMessenger::SendEvent(System.String)
			// ret
			// 
			onLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldarg_1));
			onLockMethod.Body.Instructions.Add(il.Create(
				OpCodes.Call,
				td.Methods.Single(m => m.Name == "SendEvent")));
			onLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ret));
			onLockMethod.Body.OptimizeMacros();

			return onLockMethod;
		}
			
		private static MethodDefinition proxyRequestLock(
			TypeDefinition td
		)
		{
			MethodDefinition requestLockMethod = 
				WeavingBuildingBlocks._AddMethodDefinitionToType(
					typeDefinition: td,
					nameOfMethod: "RequestLock",
					methodAttributes: MethodAttributes.Public | MethodAttributes.Static,
					returnTypeReferenceOfMethod: td.Module.Import(typeof(void))
				);
			requestLockMethod.Parameters.Add(new ParameterDefinition(td.Module.Import(typeof(string))));
			requestLockMethod.Parameters.Add(new ParameterDefinition(td.Module.Import(typeof(string))));
			requestLockMethod.Body.Variables.Add(new VariableDefinition(td.Module.Import(typeof(object))));
			VariableDefinition V_1 = new VariableDefinition(td.Module.Import(typeof(Boolean)));
			requestLockMethod.Body.Variables.Add(V_1);

			ILProcessor il = requestLockMethod.Body.GetILProcessor();
			requestLockMethod.Body.SimplifyMacros();

			// 
			// 1. lock (locking) {
			//
			// ldsfld System.Object SampleServer.BootstrapMessenger::locking
			// stloc.0
			// ldc.i4.0
			// stloc.1
			// ldloc.0
			// ldloca.s V_1
			// call System.Void System.Threading.Monitor::Enter(System.Object,System.Boolean&)
			requestLockMethod.Body.Instructions.Add(il.Create(
				OpCodes.Ldsfld,
				td.Fields.Single(f => f.Name == "locking")));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Stloc_0));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldc_I4_0));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Stloc_1));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_0));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloca_S, V_1));
			requestLockMethod.Body.Instructions.Add(il.Create(
				OpCodes.Call,
				td.Module.Import(
					typeof(System.Threading.Monitor)
					.GetMethods()
					.Where(mm => mm.Name == "Enter")
					.Single(mm => mm.GetParameters().Length == 2))));
			//Console.WriteLine("Wrote 1/3");

			// 2. OnLock(messageKey, messageValue);
			//
			// ldarg.0
			// ldarg.1
			// call System.Void SampleServer.BootstrapMessenger::OnLock(System.String,System.String)
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldarg_0));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldarg_1));
			requestLockMethod.Body.Instructions.Add(il.Create(
				OpCodes.Call,
				td.Methods.Single(m => m.Name == "OnLock")));
			//Console.WriteLine("Wrote 2/3");

			// 3. }
			// 
			// leave IL_0029
			// ldloc.1
			// brfalse.s IL_0028
			// ldloc.0
			// call System.Void System.Threading.Monitor::Exit(System.Object)
			// IL_0028: endfinally
			// IL_0029: ret
			Instruction branchDestinationEndFinally = il.Create(OpCodes.Endfinally);
			Instruction branchDestinationRet = il.Create(OpCodes.Ret);

			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Leave, branchDestinationRet));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_1));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Brfalse_S, branchDestinationEndFinally));
			requestLockMethod.Body.Instructions.Add(il.Create(OpCodes.Ldloc_0));
			requestLockMethod.Body.Instructions.Add(il.Create(
				OpCodes.Call,
				td.Module.Import(
					typeof(System.Threading.Monitor)
					.GetMethod("Exit", new [] { typeof(System.Object) }))));
			requestLockMethod.Body.Instructions.Add(branchDestinationEndFinally);
			requestLockMethod.Body.Instructions.Add(branchDestinationRet);
			//Console.WriteLine("Wrote 3/3");

			requestLockMethod.Body.OptimizeMacros();

			return requestLockMethod;
		}

		#endregion

		public static List<Instruction> pointOfInterestEventSetup(
			MethodDefinition method,
			string eventKey,
			string eventValue
		)
		{
			List<Instruction> invocationInstructions =  new List<Instruction>();

			TypeDefinition wovenFlowTestProxy =
				method.Module.Types.Single(t => t.Name == "FlowTestProxy");
			MethodDefinition RequestLock = 
				wovenFlowTestProxy.Methods.Single(m => m.Name == "RequestLock");

			invocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, eventKey));
			invocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Ldstr, eventValue));
			invocationInstructions.Add(
				method.Body.GetILProcessor().Create(
					OpCodes.Call,
					RequestLock));

			return invocationInstructions;
		}

		#region event sender

		public static void InvokeResultAggregatorBeforeMethod(
			MethodDefinition method,
			string key,
			string value
		)
		{
			List<Instruction> instructions = pointOfInterestEventSetup(method, key, value);

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodEntry(
				methodToWeave: method,
				listOfInstructionsToWeave: instructions
			);
		}

		public static void InvokeResultAggregatorAfterMethod(
			MethodDefinition method,
			string key,
			string value
		)
		{
			List<Instruction> instructions = pointOfInterestEventSetup(method, key, value);

			WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodExit (
				methodToWeave: method,
				listOfInstructionsToWeave: instructions
			);
		}
		#endregion
	}
}

