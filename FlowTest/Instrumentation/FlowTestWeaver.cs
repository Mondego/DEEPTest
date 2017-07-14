using System;
using System.Linq;
using Mono.Cecil;

namespace FlowTest
{
	public class FlowTestWeaver
	{
		private ModuleDefinition mModule; 
		private string moduleReadPath;
		private string moduleWritePath;

		public FlowTestWeaver(string inPlaceWritePath)
		{
			moduleReadPath = inPlaceWritePath;
			moduleWritePath = inPlaceWritePath;
			mModule = ModuleDefinition.ReadModule(moduleReadPath);
		
			// Credit for methodology http://einaregilsson.com/module-initializers-in-csharp/
			
		}

		public void WriteInstrumentedCodeToFile()
		{
			try
			{
				mModule.Write (moduleWritePath);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeaver.WriteInstrumentedCodeToFile() caught unexpected exception " + e.GetType() + " " + e.Message);
			}

		}

		public void WeaveWatchpointAtPointOfInterest(FlowTestPointOfInterest point)
		{
			try
			{
				WeavingAtLocation.WeaveModuleAtTargetPointCall (mModule, point);
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestWeaver.WeaveWatchpointAtPointOfInterest(poi) caught unexpected " + e.GetType() + " " + e.Message);
			}
		}
	}
}
