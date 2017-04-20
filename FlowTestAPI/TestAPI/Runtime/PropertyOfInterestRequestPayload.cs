using System;

namespace FlowTestAPI
{
	public class PropertyOfInterestRequestPayload
	{
		public Type poiType { get; set; }
		public string poiPath { get; set; }
		public object poiValue {get; set; }
	}
}

