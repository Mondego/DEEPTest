using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeepTest
{
    /// <summary>
    /// Small utility class that sends a UDP message, waits for a response, and then stops.
    /// 
    /// Probably belongs in an examples category but it's fine here for testing.
    /// </summary>
    public class UdpMessageSender
    {
        private UdpClient mClient;
        private string serverHostname;
        private int serverPort;

        public UdpMessageSender(string host, int port)
        {
            serverHostname = host;
            serverPort = port;
        }

        public void SendMessage(string send)
        {
            try {
                mClient = new UdpClient(0);
                Console.WriteLine("UDPMessageSender {0} sending {1} to {2}:{3}",
                    ((IPEndPoint)mClient.Client.LocalEndPoint).ToString(),
                    send,
                    serverHostname,
                    serverPort
                );

                IPEndPoint recvEndPoint = new IPEndPoint(IPAddress.Any, 0);

                bool listening = true;
                bool sent = false;

                while (listening)
                {
                    if (mClient.Available > 0) {
                        listening = false;
                        byte[] messageBuffer = mClient.Receive(ref recvEndPoint);
                        string received = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
                        Console.WriteLine("Received: " + received);
                    }

                    if (!sent)
                    {
                        sent = true;
                        byte[] datagram = Encoding.ASCII.GetBytes(send);
                        mClient.Send(datagram, datagram.Length, serverHostname, serverPort);
                    }
                }

            }

            catch (Exception e) {
                Console.WriteLine(e);
            }

            finally {
                Console.WriteLine("Stopping");
                mClient.Close();
            }
        }
    }
}

