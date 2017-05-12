using System;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlowTestAPI
{
	// A method which we instrument before and/or after. 
	public class FlowTestPointOfInterest
	{
		public string parentObjectOfWatchpoint { get; }
		public string methodOfInterest { get; }
		public bool watchBefore { get; set; }
		public bool watchAfter { get; set; }
		public Type mCustomWeave { get; }
		private FlowTestRuntime mRuntime;

		public FlowTestPointOfInterest (
			string parentObject,
			string methodToWatch,
			Type methodCallToWeave = null
		)
		{
			parentObjectOfWatchpoint = parentObject;
			methodOfInterest = methodToWatch;
			mCustomWeave = methodCallToWeave;

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

