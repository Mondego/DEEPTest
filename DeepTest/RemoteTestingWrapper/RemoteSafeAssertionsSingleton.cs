using System;
using System.Diagnostics;

using RemoteAssertionMessages;

using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace RemoteTestingWrapper
{
    public sealed class RemoteSafeAssertionsSingleton
    {
        private RemoteSafeAssertionsSingleton()
        {
        }

        public static RemoteSafeAssertionsSingleton Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly RemoteSafeAssertionsSingleton instance = new RemoteSafeAssertionsSingleton();
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
