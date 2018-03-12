using System;
using System.Net.Sockets;
using System.IO;
using RemoteAssertionMessages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

namespace InternalTestDriver
{
    public class AdHocInternalDriver
    {
        private TcpListener EventListener = 
            new TcpListener(
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
        private bool isListening = false;
        private Dictionary<int, BufferBlock<double>> results;

        public AdHocInternalDriver()
        {
            results = new Dictionary<int, BufferBlock<double>>();
            EventListener.Start();
            Console.WriteLine("Running AdHoc Test Driver at " + EventListener.LocalEndpoint.ToString());
            isListening = true;
            Activate();
        }

        // Obsolete this asap
        public int getDriverPort()
        {
            return ((IPEndPoint)EventListener.LocalEndpoint).Port;
        }

        public void Activate()
        {
            try
            {
                
                Task.Run(() => {
                    while (isListening) {
                        TcpClient tc = EventListener.AcceptTcpClient();
                        NetworkStream ns = tc.GetStream();
                        StreamReader sr = new StreamReader(ns);

                        string receivedJSON = sr.ReadToEnd();
                        Console.WriteLine("{Driver @ localhost:{0} <-- {1}",
                            getDriverPort(), receivedJSON);
                        AssertionResult ar = 
                            JsonConvert.DeserializeObject<AssertionResult>(receivedJSON);

                        switch (ar.assertionResultType) {
                        case "stopwatch":
                            Console.WriteLine("Stopwatch");
                            double elapsedS = (double)ar.value / 1000.0;
                            Console.WriteLine("Elapsed seconds: " + elapsedS);
                            Console.WriteLine(ar.wpKey);

                            if (!results.ContainsKey(ar.wpKey))
                            {
                                results.Add(ar.wpKey, new BufferBlock<double>());
                            }
                            results[ar.wpKey].Post((double)ar.value);

                            break;
                        default:
                            Console.WriteLine("Unknown type of assertion");
                            break;
                        }

                        ns.Close();
                        sr.Close();
                        tc.Close();
                    }
                });
            }

            catch (Exception e) {
                Console.WriteLine(e.Message + " " + e.StackTrace);
            }
        }

        public async Task<double> endToEndTime(int wpKey)
        {
            //return await results[wpKey].ReceiveAsync()
            //await 
            return 0.0;
        }

        public void Disactivate()
        {
            isListening = false;
            EventListener.Stop();
        }
    }
}

