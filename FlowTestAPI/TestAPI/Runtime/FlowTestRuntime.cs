using System;
using System.Net;

namespace FlowTestAPI
{
	public class FlowTestRuntime
	{
		private FlowTestWeaver weaver;
		private FlowTestRuntimeMothership mothership;

		private string pathToSourceExecutable;
		private string pathToWriteWovenExecutable;

		public FlowTestRuntime (string sourceExecutable, string destinationExecutable)
		{
			// This initializes the messenger for all communication between the test runtime
			// and the hook into the target component.
			mothership = new FlowTestRuntimeMothership ();

			// This initializes the weaving API, responsible for doing actual module read/writes
			//weaver = new FlowTestWeaver();
			pathToSourceExecutable = sourceExecutable;
			pathToWriteWovenExecutable = destinationExecutable;
		}

		// Tracking Properties and Points of Interest

		public void WatchProperty(FlowTestPropertyOfInterest poi)
		{
			string pathToProperty = poi.Property;
		}

		public void WatchPoint(FlowTestPointOfInterest poi)
		{
			string pathToPointOfInterest = poi.PointOfInterest;
		}

		// Weaving API
		public void Write()
		{
			FlowTestWeaver.BindModuleToTestDriver(
				modulePath: pathToSourceExecutable,
				destinationPath: pathToWriteWovenExecutable
			);
		}

		// Doing things during the test
		public void Start()
		{
		}

		public void Stop()
		{
		}

		public object GetPropertyOfInterest(string poiPath)
		{
			return mothership.GetPropertyOfInterest (poiPath);
		}
	}
}

