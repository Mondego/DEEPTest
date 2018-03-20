using System;
using System.Diagnostics;

namespace RemoteTestingWrapper
{
    public sealed class StandaloneInstrumentationMessageHandler
    {
        private StandaloneInstrumentationMessageHandler()
        {
            Console.WriteLine("First-time setup of StandaloneInstrumentationMessageHandler");
        }

        public static StandaloneInstrumentationMessageHandler Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly StandaloneInstrumentationMessageHandler instance = new StandaloneInstrumentationMessageHandler();
        }
            
        public void CaptureInstrumentationPoint(object value, string instrumentationPointId)
        {
            Console.WriteLine("Captured Object " + value);
            Console.WriteLine("Instrumentation point: " + instrumentationPointId);
        }

        public void CaptureStopwatchEndPoint(Stopwatch s, string instrumentationPointId)
        {
            Console.WriteLine("Captured Stopwatch " + s);
            Console.WriteLine("Instrumentation point: " + instrumentationPointId);
            Console.WriteLine("Elapsed ms: " + s.ElapsedMilliseconds);
        }

        public void Message(int metadata, int wpId, Stopwatch s)
        {
            /*Console.WriteLine("Sending result to 127.0.0.1:" + metadata);
            TcpClient tcpc = new TcpClient("127.0.0.1", metadata);
            NetworkStream ns = tcpc.GetStream();

            InstrumentationDataMessage sar = new InstrumentationDataMessage {
                assertionResultType = "stopwatch",
                value = s.ElapsedMilliseconds,
                wpKey = wpId
            };

            byte[] messageData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(sar));
            ns.Write(messageData, 0, messageData.Length);
            ns.Close();
            tcpc.Close();*/
        }
    }
}
