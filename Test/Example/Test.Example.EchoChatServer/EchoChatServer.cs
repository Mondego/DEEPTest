using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Test.Example.EchoChatServer
{
    public class EchoChatServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private bool listening = false;
        private UdpClient listener;

        public EchoChatServer(string host, int port)
        {
            IPAddress address = IPAddress.Parse(host);
            listener = new UdpClient(new IPEndPoint(address, port));
        }

        public override string ToString()
        {
            string listenerEndPoint = ((IPEndPoint)listener.Client.LocalEndPoint).ToString();
            return String.Format("Test.Example.EchoChatServer {0}", listenerEndPoint);
        }

        public void Start()
        {
            listening = true;
            Console.WriteLine("Starting " + this.ToString());

            try {
                while (listening) {
                    allDone.Reset();
                    listener.BeginReceive(new AsyncCallback(ReceiveMessageCallback), listener);
                    allDone.WaitOne();
                }
            }

            catch (ObjectDisposedException)
            {
                Console.WriteLine("Listener safely closed.");
            }

            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }

            finally {
                Stop();
            }
        }

        private void ReceiveMessageCallback(IAsyncResult ar)
        {
            allDone.Set();
            if (listening == false) return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            listener.BeginReceive(new AsyncCallback(ReceiveMessageCallback), listener);

            string message = Encoding.ASCII.GetString(messageBuffer);
            Console.WriteLine("{0} -> {1} : {2}", 
                endPoint.ToString(),
                ((IPEndPoint)listener.Client.LocalEndPoint).ToString(),
                message
            );

            listener.BeginSend(messageBuffer, messageBuffer.Length, endPoint, new AsyncCallback(SendMessageCallback), listener);
        }

        private void SendMessageCallback(IAsyncResult ar)
        {
            int bytesSent = ((UdpClient)ar.AsyncState).EndSend(ar);
        }

        public void Stop()
        {
            Console.WriteLine("Stopping " + this.ToString());
            listening = false;
            listener.Close();
        }
    }
}

