using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NFBenchImport.Services.ClientApplication
{
    public class ClientApplication
    {
        protected UdpClient mClient;
        protected IPAddress mServerIP;
        protected int mServerPort;
        protected string mEndpointInfo;
        protected int mId;
        protected bool running = false;

        public ClientApplication(string serverHostname, int serverPort, int id = -1)
        {
            mClient = new UdpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            mServerIP = IPAddress.Parse(serverHostname);
            mServerPort = serverPort;
            mId = id;
            mEndpointInfo = ((IPEndPoint)mClient.Client.LocalEndPoint).ToString();
        }

        public void start()
        {
            running = true;

            try {
                mClient.BeginReceive(new AsyncCallback(ReceiveAsyncMessage), mClient);

                Task.Run(() => {
                    while (running) {
                        string message = Console.ReadLine();
                        if (message.Length > 0) {
                            byte[] datagram = Encoding.ASCII.GetBytes(message);
                            //Console.WriteLine("[ClientApplication.Start #{0}] {2}",  mId, mEndpointInfo, message);
                            mClient.BeginSend(
                                datagram, 
                                datagram.Length, 
                                new IPEndPoint(mServerIP, mServerPort),
                                new AsyncCallback((IAsyncResult ar) => {
                                    //
                                }),
                                mClient
                            );
                        }
                    }
                });

                while (running)
                {
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

            finally
            {
                Console.WriteLine("Done");
                stop();
            }
        }

        protected void ReceiveAsyncMessage(IAsyncResult ar)
        {
            if (running == false)
                return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
            string messageText = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
            //Console.WriteLine("[ClientApplication.ReceiveAsyncMessage #{0}] {2}",  mId, mEndpointInfo, messageText);
            mClient.BeginReceive(new AsyncCallback(ReceiveAsyncMessage), mClient);
        }

        public void stop()
        {
            running = false;
            mClient.Close();
        }
    }
}

