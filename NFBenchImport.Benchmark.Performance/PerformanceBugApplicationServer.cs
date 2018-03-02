using System;
using System.Threading;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace NFBenchImport.Benchmark.Performance
{
    public class PerformanceBugApplicationServer
    {
        protected ConcurrentDictionary<string, IPEndPoint> mConnections;
        protected ConcurrentDictionary<int, string> mConnectionIds;
        protected bool listening = false;
        protected UdpClient listener;
        protected string mEndpointInfo;
        protected string mHostname;
        protected int mPort;

        public PerformanceBugApplicationServer (string hostname, int port)
        {
            listener = new UdpClient(new IPEndPoint(IPAddress.Parse(hostname), port));
            mConnections = new ConcurrentDictionary<string, IPEndPoint>();
            mConnectionIds = new ConcurrentDictionary<int, string>();
            mEndpointInfo = ((IPEndPoint)listener.Client.LocalEndPoint).ToString();
            mHostname = hostname;
            mPort = port;
        }

        protected void debugMessage (string message)
        {
            Console.WriteLine("{0} {1} --- {2}",
                this.GetType().Name,
                mEndpointInfo, 
                message);      
        }

        public virtual void start()
        {
            debugMessage("Listening");
            listening = true;

            try {
                listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);

                while (listening)
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
                debugMessage("Done");
                stop();
            }
        }

        protected virtual void receiveMessageCallback(IAsyncResult ar)
        {
            try
            {
                if (listening == false) return;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] messageBuffer = ((UdpClient)ar.AsyncState).EndReceive(ar, ref endPoint);
                string received = Encoding.ASCII.GetString(messageBuffer);
                Console.WriteLine("[PerformanceBugApplicationServer.receiveMessageCallback] " + received);

                mConnections.TryAdd(
                    endPoint.ToString(), 
                    new IPEndPoint(endPoint.Address, endPoint.Port));

                processMessage(received, endPoint.ToString());
            }

            catch (Exception ex) {
                debugMessage(ex.Message);
            }
        }

        protected void endMessageSendCallback(IAsyncResult ar)
        {
            int bytes = listener.EndSend(ar);
            listener.BeginReceive(new AsyncCallback(receiveMessageCallback), listener);
        }

        protected void processMessage(string message, string endp)
        {
            Thread.Sleep(1000);

            if (message[0] == '@') {
                int senderId = (int)Int64.Parse(message.Split(new char[] { ' ' }, 3)[1].Remove(0, 1));
                mConnectionIds.TryAdd(senderId, endp);
                handlePrivateMessage(message, senderId);
            }
            else
            {
                int senderId = (int)Int64.Parse(message.Split(new char[] { ' ' }, 2)[0].Remove(0, 1));
                mConnectionIds.TryAdd(senderId, endp);
                handleBroadcastMessage(message, senderId);
            }
        }

        protected void handlePrivateMessage(string message, int sentBy)
        {
            try
            {
                int pmUid = 
                    (int)Int64.Parse(
                        message.Split(new char[] { ' ' }, 2)[0]
                        .Remove(0, 1));

                if (mConnectionIds.ContainsKey(pmUid)) {
                    IPEndPoint pmDestination = mConnections[mConnectionIds[pmUid]];
                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    listener.BeginSend(
                        buffer,
                        buffer.Length,
                        pmDestination,
                        new AsyncCallback(endMessageSendCallback),
                        listener
                    );
                    debugMessage("[pm sent to] " + pmDestination.ToString());
                }
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        protected void handleBroadcastMessage(string message, int sentBy)
        {
            try 
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);

                foreach (var endp in mConnections) {
                    listener.BeginSend(
                        buffer,
                        buffer.Length,
                        endp.Value,
                        new AsyncCallback(endMessageSendCallback),
                        listener
                    );
                }
                debugMessage("[broadcasted] " + message);
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void stop()
        {
            Console.WriteLine("ReferenceApplicationServer stop");
            listening = false;
            listener.Close();
        }
    }
}

