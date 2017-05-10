using System;

namespace FlowTestAPI
{
	public class FlowTestInstrumentationEvent
	{
		public string flowParentType { get; set; }
		public string flowInstrumentationPath { get; set; }
		public int sourceFlowKey { get; set; }
		public object flowEventContent { get; set; }
	}
}

