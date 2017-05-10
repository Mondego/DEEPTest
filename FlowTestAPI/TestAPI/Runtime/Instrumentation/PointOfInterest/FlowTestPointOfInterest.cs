using System;

using Newtonsoft.Json;

namespace FlowTestAPI
{
	// A method which we instrument before and/or after. 
	public class FlowTestPointOfInterest
	{
		public string parentObjectOfWatchpoint { get; }
		public string methodOfInterest { get; }
		public bool watchBefore { get; set; }
		public bool watchAfter { get; set; }
		private FlowTestRuntime mRuntime;

		public FlowTestPointOfInterest (
			string parentObject,
			string methodToWatch
		)
		{
			parentObjectOfWatchpoint = parentObject;
			methodOfInterest = methodToWatch;

			watchBefore = true;
			watchAfter = true;
		}

		public void setRuntime(FlowTestRuntime ftr)
		{
			mRuntime = ftr;
		}

		public string generatePayload()
		{
			FlowTestInstrumentationEvent poiInfo = new FlowTestInstrumentationEvent
			{
				flowParentType = parentObjectOfWatchpoint,
				flowInstrumentationPath = methodOfInterest,
				sourceFlowKey = this.GetHashCode(),
				flowEventContent = null
			};

			return JsonConvert.SerializeObject (poiInfo, Formatting.None);
		}
	}
}

