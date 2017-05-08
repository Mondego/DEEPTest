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

		public string generatePayload()
		{
			PointofInterestPayload poiInfo = new PointofInterestPayload
			{
				poiParentObject = parentObjectOfWatchpoint,
				poiName = methodOfInterest,
				poiHashcode = this.GetHashCode(),
				poiPayloadValue = null
			};
			return JsonConvert.SerializeObject (poiInfo, Formatting.None);
		}
	}
}

