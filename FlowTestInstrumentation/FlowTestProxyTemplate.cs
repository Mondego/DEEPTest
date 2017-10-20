using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FlowTestInstrumentation
{
    public class FlowTestProxyTemplate
    {
        public static void SendEvent(int weavePointId)
        {
            string hostname = "127.0.0.1";
            int port = 60011;

            JObject ftEvent = new JObject();
            ftEvent["wpid"] = weavePointId;
            string jsonString = ftEvent.ToString();

            //Console.WriteLine("Woven SendMessage with {0}:{1} from WP#{2}", hostname, port, weavePointId);

            TcpClient tcpc = new TcpClient(hostname, port);
            NetworkStream ns = tcpc.GetStream();
            byte[] messageData = Encoding.ASCII.GetBytes(jsonString);
            ns.Write(messageData, 0, messageData.Length);
            ns.Close();
            tcpc.Close();
        }
    }
}

