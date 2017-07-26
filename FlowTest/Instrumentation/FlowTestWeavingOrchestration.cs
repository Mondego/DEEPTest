using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace FlowTest
{
	public class FlowTestWeavingOrchestration
	{
		private class ModuleToWeave
		{
			private ModuleDefinition mModule;
			private string moduleReadPath;
			private string moduleWritePath;

			public ModuleToWeave(string inPlaceWeavingPath) : this (inPlaceWeavingPath, inPlaceWeavingPath)
			{
			}

			public ModuleToWeave(string moduleSourcePath, string moduleDestinationPath)
			{
				moduleReadPath = moduleSourcePath;
				moduleWritePath = moduleDestinationPath;
				mModule = ModuleDefinition.ReadModule(moduleReadPath);
			}

			public void Write()
			{
				mModule.Write (moduleWritePath);
			}

			public void WeavePointOfInterest(FlowTestPointOfInterest point)
			{
				Weaving.WeavePointofInterest (mModule, point);
			}
		}

		private Dictionary<string, ModuleToWeave> weavesToOrchestrate;

		public FlowTestWeavingOrchestration ()
		{
			weavesToOrchestrate = new Dictionary<string, ModuleToWeave>();
		}

		public void weavePointOfInterest(FlowTestPointOfInterest point)
		{
			try
			{
				if (!weavesToOrchestrate.ContainsKey(point.parentModuleOfWatchpoint)) {
					weavesToOrchestrate.Add(
						point.parentModuleOfWatchpoint, 
						new ModuleToWeave(point.parentModuleOfWatchpoint)
					);
				}
					
				weavesToOrchestrate [point.parentModuleOfWatchpoint].WeavePointOfInterest(point);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeavingOrchestration.weavePointOfInterest(poi) caught unexpected {0} {1}",
					e.GetType(),
					e.Message);
			}
		}

		public void Write()
		{
			try
			{
				foreach (ModuleToWeave weaver in weavesToOrchestrate.Values) {
					weaver.Write();
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

