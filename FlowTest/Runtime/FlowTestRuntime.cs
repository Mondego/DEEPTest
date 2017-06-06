using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace FlowTest
{
	public class FlowTestRuntime
	{
		private FlowTestWeavingOrchestration weavingHandler;
		private FlowTestRuntimeMothership mothership;

		private TargetComponentRuntime flowTestEntryPoint;
		private string pathToFlowTestEntryPoint;

		public FlowTestRuntime (string flowTestTargetExecutable)
		{
			// This initializes the messenger for all communication between the test runtime
			// and the hook into the target component(s)
			mothership = new FlowTestRuntimeMothership ();
			//mothership.Run ();

			// This is the path to whatever component will be run to start the test, woven
			// or otherwise.
			pathToFlowTestEntryPoint = flowTestTargetExecutable;

			// The weavingHandler is the runtime's API to weaving code, validating existing weaves,
			// and any other instrumentation before runtime.
			weavingHandler = new FlowTestWeavingOrchestration();
		}
			
		public void WatchPoint(FlowTestPointOfInterest poi)
		{
			poi.setRuntime (this);
			weavingHandler.addWatchpoint(poi);
		}

		public FlowTestRuntimeMothership getLocalMessenger()
		{
			return mothership;
		}

		public void Write()
		{
			weavingHandler.Write();
		}

		// Doing things during the test

		public void ExecuteWovenWithArguments(params string[] arguments)
		{
			List<string> args = new List<string>();
			for (int i = 0; i < arguments.Length; i++) {
				args.Add(arguments [i]);
			}
			string[] targetComponentArguments = args.ToArray();

			flowTestEntryPoint = new TargetComponentRuntime (pathToFlowTestEntryPoint, targetComponentArguments);
			flowTestEntryPoint.Start();
		}

		public void Stop()
		{
			mothership.Stop ();
			flowTestEntryPoint.Stop();
		}

		public object GetPropertyOfInterest(string poiPath)
		{
			return null;
		}
	}
}

