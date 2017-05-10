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

		public void getTestResults()
		{
			Console.WriteLine("Result: " + mRuntime.getLocalMessenger ().getRuntimeFlowByKey (this.GetHashCode ())); 
		}

		public string generatePayload(object content = null)
		{
			FlowTestInstrumentationEvent poiInfo = new FlowTestInstrumentationEvent
			{
				flowParentType = parentObjectOfWatchpoint,
				flowInstrumentationPath = methodOfInterest,
				sourceFlowKey = this.GetHashCode(),
				flowEventContent = content
			};

			return JsonConvert.SerializeObject (poiInfo, Formatting.None);
		}
	}
}

