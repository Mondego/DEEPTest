using System;

namespace FlowTestAPI
{
	public class PointofInterestPayload
	{
		public string poiParentObject { get; set; }
		public string poiName { get; set; }
		public int poiHashcode { get; set; }
		public object poiPayloadValue { get; set; }
	}
}

