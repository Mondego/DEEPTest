using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

// Should I need this
using FlowTestAPI;
using System.Collections.Generic;

namespace SampleServer
{
	public class EchoServer
	{
		protected UdpClient mListener;
		protected IPEndPoint utilityEndpoint;
		protected bool running = false;
		protected int mPort;
		private int inner = 42;
		private int nMessagesSent = 0;
		private List<int> clientPorts;

		public EchoServer (int serverPort)
		{
			mPort = serverPort;
			mListener = new UdpClient(mPort);
			utilityEndpoint = new IPEndPoint(IPAddress.Any, 0);
			clientPorts = new List<int>();
		}

		public void Run()
		{
			running = true;
			Console.WriteLine("Running echo server at localhost:{0}", mPort);

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
			Console.WriteLine("[From Client] {0}", received);
			SendMessage(received, utilityEndpoint);
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

