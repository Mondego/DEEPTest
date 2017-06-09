using System;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowTest
{
	// A method which we instrument before and/or after. 
	public class FlowTestPointOfInterest
	{
		public string parentModuleOfWatchpoint { get; }
		public string parentObjectOfWatchpoint { get; }
		public string methodOfInterest { get; }
		public bool watchBefore { get; set; }
		public bool watchAfter { get; set; }
		private FlowTestRuntime mRuntime;

		public FlowTestPointOfInterest (
			string parentModule,
			string parentObject,
			string methodToWatch
		)
		{
			parentModuleOfWatchpoint = parentModule;
			parentObjectOfWatchpoint = parentObject;
			methodOfInterest = methodToWatch;

			watchBefore = true;
			watchAfter = true;
		}

		public void setRuntime(FlowTestRuntime ftr)
		{
			mRuntime = ftr;
		}

		public FlowTestInstrumentationEvent[] getTestResults()
		{
			Queue<FlowTestInstrumentationEvent> events = mRuntime.getLocalMessenger().getRuntimeFlowByKey(this.GetHashCode());
			return events.ToArray();
		}

		public FlowTestInstrumentationEvent generatePayload (object content = null)
		{
			return new FlowTestInstrumentationEvent
			{
				flowParentType = parentObjectOfWatchpoint,
				flowInstrumentationPath = methodOfInterest,
				sourceFlowKey = this.GetHashCode(),
				flowEventContent = content
			};
		}

		public string generatePayloadString (object content = null)
		{
			return JsonConvert.SerializeObject (generatePayload(content), Formatting.None);
		}
	}
}

