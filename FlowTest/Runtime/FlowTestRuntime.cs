using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlowTest
{
	public class FlowTestRuntime
	{
		private FlowTestWeavingOrchestration weavingHandler;
		private FlowTestRuntimeMothership mothership;
		List<AssemblyToExecute> flowTestStartupOrder;

		public FlowTestRuntime ()
		{
			// This initializes the messenger for all communication between the test runtime
			// and the hook into the target component(s)
			mothership = new FlowTestRuntimeMothership ();

			// The weavingHandler is the runtime's API to weaving code, validating existing weaves,
			// and any other instrumentation before runtime.
			weavingHandler = new FlowTestWeavingOrchestration();

			// The flowTestStartupOrder list is all services that need to be started. It's a directed
			// graph of sorts, though very simple at the moment. Designed for systems that have
			// complex startup sequences and/or delays.
			flowTestStartupOrder = new List<AssemblyToExecute>();
		}

		public void Start()
		{
			foreach (AssemblyToExecute flowExecutionComponent in flowTestStartupOrder) {
				flowExecutionComponent.Start();
			}
			mothership.Run ();
		}
			
		public void addAssemblyToFlowTest(string pathToAssembly, int nSecondsRequiredAfterLaunch, string[] args)
		{
			flowTestStartupOrder.Add(
				new AssemblyToExecute(
					assemblyExecutionPath: pathToAssembly,
					nSecondsForStartup: nSecondsRequiredAfterLaunch,
					arguments: args
				)
			);
		}

		public void AddWatchPoint(FlowTestPointOfInterest poi)
		{
			try {
				poi.setRuntime (this);
				weavingHandler.addWatchpoint(poi);
			} catch (Exception e) {
				Console.WriteLine("FlowTestRuntime.WatchPoint(poi) unexpected exception " + e.GetType() + " " + e.Message);		
			}
		}

		public void Stop()
		{
			try 
			{
				mothership.Stop ();
				for (int componentIndex = flowTestStartupOrder.Count - 1; componentIndex >= 0; componentIndex--)
				{
					flowTestStartupOrder[componentIndex].Stop();
				}
			}

			catch (ArgumentException ae)
			{
				// Process not found
			}

			catch (InvalidOperationException ioe) 
			{
				// Process couldn't be stopped cause it probably terminated due to a weaving error
			}

			catch (Exception e) {
				Console.WriteLine("FlowTestRuntime.Stop caught unexpected exception " + e.GetType() + " " + e.Message);
			}
		}

		public FlowTestRuntimeMothership getLocalMessageHandler()
		{
			return mothership;
		}

		public void Write()
		{
			try {
				weavingHandler.Write();
			} catch (Exception e) {
				Console.WriteLine("FlowTestRuntime.Write() unexpected exception " + e.GetType() + " " + e.Message);		
			}
		}
	}
}

