using System;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections;

namespace FlowTest
{
	// A method which we instrument before and/or after. 
	public class FlowTestPointOfInterest
	{
		public string parentModuleOfWatchpoint { get; }
		public string parentTypeOfWatchpoint { get; }
		public string methodOfInterest { get; }
		public bool watchBefore { get; set; }
		public bool watchAfter { get; set; }
		public string content { get; set; }

		private bool sendEvents = true;
		private FlowTestRuntime mRuntime;

		public FlowTestPointOfInterest (
			string parentModule,
			string parentType,
			string methodToWatch
		)
		{
			parentModuleOfWatchpoint = parentModule;
			parentTypeOfWatchpoint = parentType;
			methodOfInterest = methodToWatch;

			watchBefore = true;
			watchAfter = true;
			content = "";
		}

		public void willSendEvents(bool setValue)
		{
			sendEvents = false;
		}

		public void setRuntime(FlowTestRuntime ftr)
		{
			mRuntime = ftr;
		}

		public NameValueCollection[] getTestResults()
		{
			Queue<NameValueCollection> events = mRuntime.getEventHandler().getAggregationByKey(this.GetHashCode());
			return events.ToArray();
		}

		public NameValueCollection getNextEventResult()
		{
			return mRuntime.getEventHandler().getNextQueuedEventByKey(this.GetHashCode());
		}

		public string generatePayload(string content = "")
		{
			List<KeyValuePair<string, string>> eventContents = new List<KeyValuePair<string, string>>();

			eventContents.Add(new KeyValuePair<string, string>("key", this.GetHashCode().ToString()));
			eventContents.Add(new KeyValuePair<string, string>("parentModuleName", parentModuleOfWatchpoint));
			eventContents.Add(new KeyValuePair<string, string>("parentTypeName", parentTypeOfWatchpoint));
			eventContents.Add(new KeyValuePair<string, string>("point", methodOfInterest));
			eventContents.Add(new KeyValuePair<string, string>("value", content));

			return new FormUrlEncodedContent(eventContents).ReadAsStringAsync().Result;
		}

		public void weaveIntoModule(
			ModuleDefinition module
		)
		{
			if (!sendEvents) {
				return;
			}
			try {
				TypeDefinition destinationType = module.Types.Single(t => t.Name == parentTypeOfWatchpoint);
				MethodDefinition poiMethod = destinationType.Methods.Single(m => m.Name == methodOfInterest);

				if (watchBefore)
				{
					WeavingFlowTestProxy.InvokeResultAggregatorBeforeMethod(
						method: poiMethod,
						key: GetHashCode().ToString(),
						value: generatePayload(content: "before")
					);

					WeavingDebug.WeaveDebugStatementBeforeMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened before " + methodOfInterest
					);
				}

				if (watchAfter)
				{
					WeavingFlowTestProxy.InvokeResultAggregatorAfterMethod(
						method: poiMethod,
						key: GetHashCode().ToString(),
						value: generatePayload(content: "after")
					);

					WeavingDebug.WeaveDebugStatementAfterMethod(
						targetMethod: poiMethod,
						printDebugValue: "Some weaving happened after " + methodOfInterest
					);
				}
			}

			catch (Exception e) {
				Console.WriteLine ("| FlowTest Weaver caught an exception while adding a point of interest.");
				Console.WriteLine ("| PoI: {0} => {1}", parentTypeOfWatchpoint, methodOfInterest);
				Console.WriteLine ("| {0} {1}", e.InnerException, e.Message);
			}
		}
	
		public void aLaCarteSystemCallBefore(
			Type systemType,
			string methodName,
			object[] parameters
		)
		{
			try 
			{
				ModuleDefinition module = mRuntime.getWeavingHandler().getPoiModule(this);
				TypeDefinition destinationType = module.Types.Single(t => t.Name == parentTypeOfWatchpoint);
				MethodDefinition poiMethod = destinationType.Methods.Single(m => m.Name == methodOfInterest);
				ILProcessor proc = poiMethod.Body.GetILProcessor();

				Console.WriteLine("???");
				List<Instruction> instructions = new List<Instruction>();
				List<Type> args = new List<Type>();
			
				foreach(object o in parameters)
				{
					if (o.GetType() == typeof(int))
					{
						instructions.Add(proc.Create(OpCodes.Ldc_I4, (int)o));
						args.Add(typeof(int));
					}

					else if (o.GetType() == typeof(string))
					{
						instructions.Add(proc.Create(OpCodes.Ldstr, (string)o));
						args.Add(typeof(string));
					}
				}
					
				instructions.Add(proc.Create(
					OpCodes.Call,
					module.Import(systemType.GetMethod(methodName, args.ToArray()))));

				Console.WriteLine("???");

				WeavingBuildingBlocks.WeaveListOfInstructionsAtMethodEntry(
					methodToWeave: poiMethod,
					listOfInstructionsToWeave: instructions
				);
			}

			catch (Exception e) {
				Console.WriteLine ("| FlowTest Weaver caught an exception while adding alacarte before.");
				Console.WriteLine ("| PoI: {0} => {1}", parentTypeOfWatchpoint, methodOfInterest);
				Console.WriteLine ("| {0} {1}", e.InnerException, e.Message);
			}
		}

		public void aLaCarteSystemCallAfter(
			Type systemType,
			string methodName,
			Object[] parameters
		)
		{
			//
		}
	}
}

