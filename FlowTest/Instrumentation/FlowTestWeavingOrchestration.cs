using System;
using System.Collections.Generic;

namespace FlowTest
{
	public class FlowTestWeavingOrchestration
	{
		private Dictionary<string, FlowTestWeaver> weavesToOrchestrate;

		public FlowTestWeavingOrchestration ()
		{
			weavesToOrchestrate = new Dictionary<string, FlowTestWeaver>();
		}

		public void addWatchpoint(FlowTestPointOfInterest point)
		{
			if (!weavesToOrchestrate.ContainsKey(point.parentModuleOfWatchpoint)) {
				weavesToOrchestrate.Add(
					point.parentModuleOfWatchpoint, 
					new FlowTestWeaver(point.parentModuleOfWatchpoint, point.parentModuleOfWatchpoint)
				);
			}

			weavesToOrchestrate [point.parentModuleOfWatchpoint].WeaveWatchpointAtPointOfInterest(point);
		}

		public void Write()
		{
		}
	}
}

