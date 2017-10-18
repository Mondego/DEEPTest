using System;

namespace FlowTestInstrumentation
{
    public class FlowTestProxyTemplate
    {
        private static readonly object _lock = new object();
        public static void RequestLock()
        {
            object locking; 

            lock (_lock) {
                //SendEvent();
            }
        }

        public static void SendEvent(ref string wp)
        {
            string hostname = "127.0.0.1";
            int port = 60011;

            Console.WriteLine("Woven SendMessage with {0}:{1} from WP#{2}", hostname, port, wp);

            /*TcpClient tcpc = new TcpClient(hostname, port);
            NetworkStream ns = tcpc.GetStream();
            byte[] messageData = Encoding.ASCII.GetBytes(message);
            ns.Write(messageData, 0, messageData.Length);
            ns.Close();
            tcpc.Close();*/
        }
    }
}

