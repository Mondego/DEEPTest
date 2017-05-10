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

		public FlowTestRuntime (string sourceExecutable, string destinationExecutable)
		{
			// This initializes the messenger for all communication between the test runtime
			// and the hook into the target component.
			mothership = new FlowTestRuntimeMothership ();
			mothership.Run ();

			// This initializes the weaving API, responsible for doing actual module read/writes
			pathToSourceExecutable = sourceExecutable;
			pathToWriteWovenExecutable = destinationExecutable;
			weaver = new FlowTestWeaver (
				sourceModulePath: pathToSourceExecutable,
				destinationModulePath: pathToWriteWovenExecutable
			);

			// For now, we don't want to hold on to previously woven executables
			if (File.Exists(pathToWriteWovenExecutable)) {
				Console.WriteLine("Deleting stale instrumented file {0}", pathToWriteWovenExecutable);
				File.Delete(pathToWriteWovenExecutable);
			}
		}

		public string getSourceComponentPath()
		{
			return pathToSourceExecutable;
		}

		public string getDestinationComponentPath()
		{
			return pathToWriteWovenExecutable;
		}

		// Tracking Properties and Points of Interest
		// TODO 
		public void WatchProperty(FlowTestPropertyOfInterest poi)
		{
			string pathToProperty = poi.Property;
		}

		public void WatchPoint(FlowTestPointOfInterest poi)
		{
			poi.setRuntime (this);
			weaver.WeaveWatchpointAtPointOfInterest (poi);
		}

		public FlowTestRuntimeMothership getLocalMessenger()
		{
			return mothership;
		}

		// Weaving API
		public void Write()
		{
			weaver.WriteInstrumentedCodeToFile ();
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
			mothership.Stop ();
			wovenComponent.Stop();
		}

		public object GetPropertyOfInterest(string poiPath)
		{
			return null;
		}
	}
}

