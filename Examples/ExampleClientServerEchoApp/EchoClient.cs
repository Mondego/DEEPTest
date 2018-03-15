using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ExampleClientServerEchoApp
{
    public class EchoClient
    {
        private UdpClient mClient;
        private string mServerHostname;
        private int mServerPort;
        private bool listening = false;
        private IPEndPoint remoteEndPoint;

        public EchoClient(int serverPort)
        {
            mServerHostname = "127.0.0.1";
            mServerPort = serverPort;
            mClient = new UdpClient(0);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void Start()
        {
            listening = true;
          
            Task.Run(() => {
                while (listening)
                {
                    Console.WriteLine("Enter a message to send:");
                    string message = Console.ReadLine();
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    mClient.Send(buffer, buffer.Length, mServerHostname, mServerPort);
                    Console.WriteLine("Sent: " + message);
                }
            });

            while (listening) {
                if (mClient.Available > 0) {
                    byte[] encoded = mClient.Receive(ref remoteEndPoint);
                    string received = Encoding.UTF8.GetString(encoded);
                    Console.WriteLine("Received: " + received);
                }
            }
        }

        public void Stop()
        {
            listening = false;
            mClient.Close();
        }
    }
}

