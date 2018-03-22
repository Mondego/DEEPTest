using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteAssertionMessages;

namespace DeepTestFramework
{
    // This is separate so it can be optimized more easily later
    public class SystemUnderTestMessageHandler
    {
        private TcpListener listener; 
        private bool isListening = false;

        public SystemUnderTestMessageHandler()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            Start();
        }

        public void Start()
        {
            listener.Start();
            isListening = true;

            Task.Run(() => {
                while (isListening) {
                    using (TcpClient tc = listener.AcceptTcpClient())
                    using (NetworkStream ns = tc.GetStream())
                    using (StreamReader sr = new StreamReader(ns))
                    {
                        string receivedJSON = sr.ReadToEnd();

                        InstrumentationPointExchangeMessage m =
                            JsonConvert.DeserializeObject<InstrumentationPointExchangeMessage>(receivedJSON);
                        Console.WriteLine(m);      
                    }
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

