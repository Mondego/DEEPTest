using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace FlowTestAPI
{
	public class FlowTestRuntime
	{
		private FlowTestWeaver weaver;
		private FlowTestRuntimeMothership mothership;
		private TargetComponentRuntime wovenComponent;

		private string pathToSourceExecutable;
		private string pathToWriteWovenExecutable;

		public string SourcePath 
		{ 
			get { return pathToSourceExecutable; }
		}

		public string InstrumentedPath
		{
			get { return pathToWriteWovenExecutable; }
		}
			
		public FlowTestRuntime (string sourceExecutable, string destinationExecutable)
		{
			// This initializes the messenger for all communication between the test runtime
			// and the hook into the target component.
			mothership = new FlowTestRuntimeMothership ();

			// This initializes the weaving API, responsible for doing actual module read/writes
			//weaver = new FlowTestWeaver();
			pathToSourceExecutable = sourceExecutable;
			pathToWriteWovenExecutable = destinationExecutable;

			if (File.Exists(pathToWriteWovenExecutable)) {
				Console.WriteLine("Deleting stale instrumented file {0}", pathToWriteWovenExecutable);
				File.Delete(pathToWriteWovenExecutable);
			}
		}

		// Tracking Properties and Points of Interest

		public void WatchProperty(FlowTestPropertyOfInterest poi)
		{
			string pathToProperty = poi.Property;
		}

		public void WatchPoint(FlowTestPointOfInterest poi)
		{
			string pathToPointOfInterest = poi.PointOfInterest;

			if (poi.watchAfter) {

			}

			if (poi.watchBefore) {

			}
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
		public void ExecuteWovenWithArguments(params string[] arguments)
		{
			List<string> args = new List<string>();
			for (int i = 0; i < arguments.Length; i++) {
				args.Add(arguments [i]);
			}
			string[] targetComponentArguments = args.ToArray();

			wovenComponent = new TargetComponentRuntime (pathToWriteWovenExecutable, targetComponentArguments);
			wovenComponent.Start();
		}

		public void Stop()
		{
			wovenComponent.Stop();
		}

		public object GetPropertyOfInterest(string poiPath)
		{
			return mothership.GetPropertyOfInterest (poiPath);
		}
	}
}

