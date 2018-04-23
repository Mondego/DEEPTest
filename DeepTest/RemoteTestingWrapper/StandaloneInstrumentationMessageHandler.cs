using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteAssertionMessages;

namespace RemoteTestingWrapper
{
    public sealed class StandaloneInstrumentationMessageHandler
    {
        private Dictionary<string, WovenSnapshot> snapshots;
        private Dictionary<string, List<long>> watches;
        private UdpClient listener;

        private static readonly StandaloneInstrumentationMessageHandler instance = new StandaloneInstrumentationMessageHandler();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static StandaloneInstrumentationMessageHandler()
        {
        }

        private StandaloneInstrumentationMessageHandler()
        {
            Console.WriteLine("First-time setup of StandaloneInstrumentationMessageHandler");
            snapshots = new Dictionary<string, WovenSnapshot>();
            watches = new Dictionary<string, List<long>>();

            listener = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), RemoteConfigurations.RemoteListenerPort));
            listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
        }

        public static StandaloneInstrumentationMessageHandler Instance
        {
            get
            {
                return instance;
            }
        }

        private void receiveMessageCallback(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            string decoded = Encoding.UTF8.GetString(messageBuffer);

            Console.WriteLine("decoded: " + decoded);

            InstrumentationPointExchangeMessage m =
                JsonConvert.DeserializeObject<InstrumentationPointExchangeMessage>(decoded);

            if (snapshots.ContainsKey(m.instrumentationPointName))
            {
                m.value = snapshots[m.instrumentationPointName].Value;
            }

            if (watches.ContainsKey(m.instrumentationPointName)) {
                m.value = watches[m.instrumentationPointName][watches[m.instrumentationPointName].Count - 1];
            }

            string responseString = JsonConvert.SerializeObject(m, Formatting.Indented);
            Console.WriteLine("Replying with: " + responseString);
            byte[] responseData = Encoding.UTF8.GetBytes(responseString);
            listener.Send(responseData, responseData.Length, endPoint);
            listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
        }

        public void CaptureInstrumentationPoint(object value, string instrumentationPointId)
        {
            Console.WriteLine("Captured Snapshot {0} val: {1}", instrumentationPointId, value);
            if (snapshots.ContainsKey(instrumentationPointId))
            {
                snapshots[instrumentationPointId].Update(value);
            }
        }

        public void CaptureStopwatchEndPoint(Stopwatch s, string instrumentationPointId)
        {
            Console.WriteLine("Captured Stopwatch {0} t: {1}", instrumentationPointId, s.ElapsedMilliseconds);
            if (!watches.ContainsKey(instrumentationPointId))
            {
                watches.Add(instrumentationPointId, new List<long>());
            }

            watches[instrumentationPointId].Add(s.ElapsedMilliseconds);
        }

        public void Bootstrap()
        {
            Console.WriteLine("Eager instantiation");
        }

        public void Register(WovenSnapshot injectedField)
        {
            snapshots.Add(injectedField.Name, injectedField);
        }

        public object getSnapshot(string ipName)
        {
            if (!snapshots.ContainsKey(ipName))
            {
                return null;
            }

            return snapshots[ipName].Value;
        }
    }
}
