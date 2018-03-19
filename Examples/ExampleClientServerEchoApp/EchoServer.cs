using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ExampleClientServerEchoApp
{
    public class EchoServer
    {
        private UdpClient listener;
        private bool listening = false;
        private int mPort;
        private IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        private int nMessagesReceived = 0;
        private int nMessagesSent = 0;

        public EchoServer(int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
            mPort = port;
        }

        public void Start()
        {
            listening = true;
            Console.WriteLine("EchoServer listening @ localhost:" + mPort);
            while (listening) {
                if (listener.Available > 0) {
                    string message = GetAvailableMessage();
                    RespondToMessage(remote, message);
                }
            }
        }

        public string GetAvailableMessage()
        {
            byte[] encoded = listener.Receive(ref remote);
            nMessagesReceived += 1;
            return Encoding.UTF8.GetString(encoded);
        }

        public void RespondToMessage(IPEndPoint sendTo, string message)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes("echo " + message);
            listener.Send(responseBuffer, responseBuffer.Length, sendTo);
            nMessagesSent += 1;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping EchoServer @ {0}", mPort);
            listening = false;
            listener.Close();
        }
    }
}

