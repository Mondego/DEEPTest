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
			try
			{
				if (!weavesToOrchestrate.ContainsKey(point.parentModuleOfWatchpoint)) {
					weavesToOrchestrate.Add(
						point.parentModuleOfWatchpoint, 
						new FlowTestWeaver(point.parentModuleOfWatchpoint, point.parentModuleOfWatchpoint)
					);
				}
					
				weavesToOrchestrate [point.parentModuleOfWatchpoint].WeaveWatchpointAtPointOfInterest(point);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.addWatchpoint(poi) caught unexpected {0} {1}",
					e.GetType(),
					e.Message);
			}
		}

		public void Write()
		{
			try
			{
				foreach (FlowTestWeaver weaver in weavesToOrchestrate.Values) {
					weaver.WriteInstrumentedCodeToFile();
				}
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.Write() caught unexpected {0} {1}",
					e.GetType(),
					e.Message);
			}
		}
	}
}

