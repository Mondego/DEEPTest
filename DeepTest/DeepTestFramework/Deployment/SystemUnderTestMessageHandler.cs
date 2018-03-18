using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using RemoteAssertionMessages;

using Newtonsoft.Json;

namespace DeepTestFramework
{
    // This is separate so it can be optimized more easily later
    public class SystemUnderTestMessageHandler
    {
        public int Port { get; private set; }
        private TcpListener listener; 
        private bool isListening = false;

        public SystemUnderTestMessageHandler()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            Port = ((IPEndPoint)listener.LocalEndpoint).Port;
           
            Start();
        }

        public void Start()
        {
            listener.Start();
            isListening = true;

            Task.Run(() => {
                while (isListening) {
                    Console.WriteLine("waiting...");
                    TcpClient tc = listener.AcceptTcpClient();
                    NetworkStream ns = tc.GetStream();
                    StreamReader sr = new StreamReader(ns);
                    string receivedJSON = sr.ReadToEnd();

                    InstrumentationDataMessage m =
                        JsonConvert.DeserializeObject<InstrumentationDataMessage>(receivedJSON);
                    Console.WriteLine(m);

                    ns.Close();
                    sr.Close();
                    tc.Close();                
                }
            });
        }

        public void Stop()
        {
            isListening = false;
            listener.Stop();
        }
    }
}

