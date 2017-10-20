using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace FlowTest
{	
	public class FlowTestEventAggregator
	{
        private IPAddress mAddress = IPAddress.Any;
        private int mPort = 60011; // TODO SHOULD BE CONFIGURABLE
		private TcpListener listener;
        private Dictionary<int, List<FlowTestEvent>> mapWeavePointToEventList = new Dictionary<int, List<FlowTestEvent>>();
        private volatile bool alive = false;

        public FlowTestEventAggregator ()
        {
            listener = new TcpListener(mAddress, mPort);
        }

		public void Start ()
        {
            Console.WriteLine("Starting event handler");

            Task.Run(() => {
                listener.Start();
                alive = true;

                try {
                    while (alive) {
                        using (TcpClient tc = listener.AcceptTcpClient())
                        {
                            NetworkStream ns = tc.GetStream ();
                            StreamReader sr = new StreamReader (ns);
                            string data = sr.ReadToEnd();

                            JObject receivedEvent = JObject.Parse(data);
                            int weavePointId = receivedEvent["wpid"].ToObject<int>();

                            Console.WriteLine("Received event: " + data);

                            if (!mapWeavePointToEventList.ContainsKey(weavePointId))
                            {
                                Console.WriteLine("Adding event log for WP#{0}", receivedEvent["wpid"]);
                                mapWeavePointToEventList[weavePointId] = new List<FlowTestEvent>();
                            }
                            mapWeavePointToEventList[weavePointId].Add(new FlowTestEvent { wpid = weavePointId });

                            ns.Close ();
                            sr.Close ();
                        }
                    }
                } catch (ThreadAbortException tae)
                {
                    // TODO this happens if test too short
                } catch (Exception e) {
                    Console.WriteLine("FlowTestEventAggregator {0} {1}", e.GetType(), e.Message);
                } finally {
                    listener.Server.Close();
                    listener.Stop();
                }
            });
        }
        
		public void Stop()
		{
            Console.WriteLine("Stopping event handler");
            alive = false;
            Thread.Sleep(2000);
		}
	}
}

