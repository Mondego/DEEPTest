using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FlowTest
{	
	// Cheesy name, handles the nitty-gritty of endpoints, content messages, etc 
	public class FlowTestRuntimeMothership
	{
		private FlowTestRuntimeConnection MothershipEndpoint;
		private FlowTestRuntimeConnection AwayTeamComponentEndpoint;
		private TcpListener MothershipMessageListener;
		private Dictionary<int, Queue<FlowTestInstrumentationEvent>> testRuntimeEvents;
		private bool isListening = false;

		// TODO - Shouldn't be hardcoded
		private int defaultMothershipEndpointPort = 60011;
		private int defaultAwayTeamEndpointPort = 60012;

		// Default for now is we're just going to weave stuff into one target component per test runtime.
	    // e.g., a mapping of one NUnit test suite to one FlowTestRuntime, and consequently one target
		// component. No idea if this is accurate long-term but seems organized enough.
		public FlowTestRuntimeMothership (int quantityTargetComponents = 1)
		{
			MothershipEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, defaultMothershipEndpointPort);
			AwayTeamComponentEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, defaultAwayTeamEndpointPort);
			MothershipMessageListener = new TcpListener(MothershipEndpoint.Address, MothershipEndpoint.Port);
			testRuntimeEvents = new Dictionary<int, Queue<FlowTestInstrumentationEvent>> ();
		}

		public Queue<FlowTestInstrumentationEvent> getRuntimeFlowByKey(int flowKey)
		{
			if (testRuntimeEvents.ContainsKey (flowKey)) {
				return testRuntimeEvents [flowKey];
			} else {
				return null;
			}
		}

		public void Run ()
		{
			isListening = true;

			Task.Run (() => {
				try {
					MothershipMessageListener.Start ();
					while (isListening) {
						TcpClient tc = MothershipMessageListener.AcceptTcpClient ();
						NetworkStream ns = tc.GetStream ();
						StreamReader sr = new StreamReader (ns);

						string receivedJSON = sr.ReadToEnd ();
						Console.WriteLine ("[DEBUG localhost:{0} received result]: {1}", MothershipEndpoint.Port, receivedJSON); 

						FlowTestInstrumentationEvent eventFromWovenComponent =
							JsonConvert.DeserializeObject<FlowTestInstrumentationEvent> (receivedJSON);

						if (!testRuntimeEvents.ContainsKey (eventFromWovenComponent.sourceFlowKey)) {
							testRuntimeEvents.Add (
								eventFromWovenComponent.sourceFlowKey,
								new Queue<FlowTestInstrumentationEvent> ()
							);
						}
						testRuntimeEvents [eventFromWovenComponent.sourceFlowKey].Enqueue (eventFromWovenComponent);

						ns.Close ();
						sr.Close ();
						tc.Close ();
					}
				} catch (Exception e) {
					Console.WriteLine ("Flow Test Mothership caught an exception: " + e.Message);
				}
			});
		}

		public void Stop()
		{
			isListening = false;
			MothershipMessageListener.Stop ();
		}
	}
}

