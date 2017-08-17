using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Web;

namespace FlowTest
{	
	// Cheesy name, handles the nitty-gritty of endpoints, content messages, etc 
	public class FlowTestEventAggregator
	{
		private FlowTestRuntimeConnection MothershipEndpoint;
		private TcpListener EventListener;
		private Dictionary<int, Queue<NameValueCollection>> testRuntimeEvents;
		private bool isListening = false;

		private int defaultMothershipEndpointPort = 60011;

		// Default for now is we're just going to weave stuff into one target component per test runtime.
	    // e.g., a mapping of one NUnit test suite to one FlowTestRuntime, and consequently one target
		// component. No idea if this is accurate long-term but seems organized enough.
		public FlowTestEventAggregator ()
		{
			MothershipEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, defaultMothershipEndpointPort);
			EventListener = new TcpListener(MothershipEndpoint.Address, MothershipEndpoint.Port);
			testRuntimeEvents = new Dictionary<int, Queue<NameValueCollection>> ();
		}

		public Queue<NameValueCollection> getAggregationByKey(int flowKey)
		{
			if (testRuntimeEvents.ContainsKey (flowKey)) {
				return testRuntimeEvents [flowKey];
			} else {
				return null;
			}
		}

		public NameValueCollection getNextQueuedEventByKey(int flowKey)
		{
			if (testRuntimeEvents.ContainsKey (flowKey) && testRuntimeEvents[flowKey].Count > 0)
			{
				return testRuntimeEvents[flowKey].Dequeue();
			}

			return null;
		}

		public void Run ()
		{
			Console.WriteLine("Starting event aggregator");
			isListening = true;

			Task.Run (() => {
				try {
					EventListener.Start ();
					Console.WriteLine("Starting FlowTest event listener at localhost:{0}", defaultMothershipEndpointPort);
					while (isListening) {
						TcpClient tc = EventListener.AcceptTcpClient ();
						NetworkStream ns = tc.GetStream ();
						StreamReader sr = new StreamReader (ns);

						string receivedKeyValueEvent = sr.ReadToEnd ();

						NameValueCollection eventProperties = HttpUtility.ParseQueryString(receivedKeyValueEvent);
						foreach (string key in eventProperties.Keys)
						{
							// Console.WriteLine("EVENT [key: {0}, value: {1}]", key, eventProperties[key]);
						}


						if (!testRuntimeEvents.ContainsKey (Int32.Parse(eventProperties["key"]))) {
							testRuntimeEvents.Add (
								Int32.Parse(eventProperties["key"]),
								new Queue<NameValueCollection>()
							);
						}
						testRuntimeEvents [Int32.Parse(eventProperties["key"])].Enqueue (eventProperties);

						ns.Close ();
						sr.Close ();
						tc.Close ();
					}
				} 

				catch (SocketException se)
				{
					// TODO this is going to happen when the test ends early.
				}

				catch (Exception e) {
					Console.WriteLine ("Flow Test Mothership caught an exception: " + e.Message + e.GetType());
				}
			});
		}

		public void Stop()
		{
			isListening = false;
			Thread.Sleep(1000);
			Console.WriteLine("Stopping FlowTest event listener at localhost:{0}", defaultMothershipEndpointPort);
			EventListener.Stop ();
		}
	}
}

