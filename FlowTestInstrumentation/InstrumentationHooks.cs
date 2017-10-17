using System;

namespace FlowTestInstrumentation
{
    public static class InstrumentationHooks
    {
        public static void SendEvent()
        {
            string hostname = "127.0.0.1";
            int port = 60011;

            Console.WriteLine("Woven SendMessage with {0}:{1} from WP#??", hostname, port);

            /*TcpClient tcpc = new TcpClient(hostname, port);
            NetworkStream ns = tcpc.GetStream();
            byte[] messageData = Encoding.ASCII.GetBytes(message);
            ns.Write(messageData, 0, messageData.Length);
            ns.Close();
            tcpc.Close();*/
        }
    }
}

