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
	public class WeavePoint
	{
		public string moduleReadPath { get; }
		public string parentTypeOfWatchpoint { get; }
		public string methodOfInterest { get; }

		public WeavePoint (
			string parentModule,
			string parentType,
			string methodToWatch
		)
		{
			moduleReadPath = parentModule;
			parentTypeOfWatchpoint = parentType;
			methodOfInterest = methodToWatch;
		}
            
        // TODO
		public string generatePayload(string content = "")
		{
			List<KeyValuePair<string, string>> eventContents = new List<KeyValuePair<string, string>>();

			eventContents.Add(new KeyValuePair<string, string>("key", this.GetHashCode().ToString()));
			eventContents.Add(new KeyValuePair<string, string>("parentModuleName", moduleReadPath));
			eventContents.Add(new KeyValuePair<string, string>("parentTypeName", parentTypeOfWatchpoint));
			eventContents.Add(new KeyValuePair<string, string>("point", methodOfInterest));
			eventContents.Add(new KeyValuePair<string, string>("value", content));

			return new FormUrlEncodedContent(eventContents).ReadAsStringAsync().Result;
		}

        // TODO
		public void weaveIntoModule(
			ModuleDefinition module
		)
		{
			/*try {
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
			}*/
		}
	}
}

