using System;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;

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
		}

		public void setRuntime(FlowTestRuntime ftr)
		{
			mRuntime = ftr;
		}

		public string[] getTestResults()
		{
			Queue<string> events = mRuntime.getLocalMessageHandler().getAggregationByKey(this.GetHashCode());
			return events.ToArray();
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
	}
}

