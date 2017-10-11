using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace FlowTest
{	
	public class FlowTestEventAggregator
	{
        private FlowTestRuntimeConnection EventHandlerEndpoint;
		private TcpListener listener;
		private Dictionary<int, Queue<NameValueCollection>> testRuntimeEvents;
        private volatile bool alive = false;

        public FlowTestEventAggregator ()
        {
            //Console.WriteLine("Initializing FlowTest event listener at localhost:{0}", port);
			EventHandlerEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, 60011);
			listener = new TcpListener(EventHandlerEndpoint.Address, EventHandlerEndpoint.Port);
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

		public void Start ()
        {
            Console.WriteLine("Starting event handler");

            Task.Run(() => {
                listener.Start();
                alive = true;

                try {
                    while (alive) {
                        TcpClient tc = listener.AcceptTcpClient();
                        NetworkStream ns = tc.GetStream ();
                        StreamReader sr = new StreamReader (ns);
                        string data = sr.ReadToEnd();

                        NameValueCollection eventProperties = HttpUtility.ParseQueryString(data);
                        foreach (string key in eventProperties.Keys)
                        {
                            Console.WriteLine("EVENT [key: {0}, value: {1}]", key, eventProperties[key]);
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
                        tc.Close();
                    }
                } catch (Exception e) {
                    Console.WriteLine("FlowTestEventAggregator {0} {1}", e.GetType(), e.Message);
                }
            });
        }
        
		public void Stop()
		{
            try
            {
                Console.WriteLine("Stopping event handler");
			    alive = false;
                listener.Server.Close();
                listener.Stop();
            }

            catch (Exception e) {
                Console.WriteLine("FlowTestRuntime.Stop caught unexpected exception " + e.GetType() + " " + e.Message);
            }
		}
	}
}

