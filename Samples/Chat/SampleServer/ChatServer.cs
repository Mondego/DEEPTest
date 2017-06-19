using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace SampleServer
{
	public class ChatServer
	{
		protected UdpClient mListener;
		protected IPEndPoint utilityEndpoint;
		protected bool running = false;
		protected int mPort;
		private int nMessagesSent = 0;
		private List<int> clientPorts;

		public ChatServer (int serverPort)
		{
			mPort = serverPort;
			mListener = new UdpClient(mPort);
			utilityEndpoint = new IPEndPoint(IPAddress.Any, 0);
			clientPorts = new List<int>();
		}

		public void Run()
		{
			running = true;
			Console.WriteLine("Running chat server at localhost:{0}", mPort);

			while (running == true) {
				if (mListener.Available > 0) {
					ReceiveMessage();
				}
			}
		}

		protected void ReceiveMessage( )
		{
			byte[] messageBuffer = mListener.Receive(ref utilityEndpoint);
			string received = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
			Console.WriteLine("[Server localhost:{0}][From Client localhost:{1}] {2}", mPort, utilityEndpoint.Port, received);

			if (!clientPorts.Contains (utilityEndpoint.Port)) {
				clientPorts.Add (utilityEndpoint.Port);
			}

			foreach (int localClientPort in clientPorts)
			{
				SendMessage(received, new IPEndPoint(IPAddress.Any, localClientPort));
			}
			//Console.WriteLine("test");
		}

		protected void SendMessage(string messageText, IPEndPoint destination)
		{
			byte[] encodedMessage = Encoding.ASCII.GetBytes(messageText);
			mListener.Client.SendTo(encodedMessage, destination);
			nMessagesSent += 1;
		}

		public void Stop()
		{
			running = false;
			mListener.Close();
		}
	}
}
