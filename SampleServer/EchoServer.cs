using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

// Should I need this
using FlowTestAPI;

namespace SampleServer
{
	public class EchoServer
	{
		protected UdpClient mListener;
		protected bool running = false;
		protected int mPort;
		private int inner = 42;

		public EchoServer (int serverPort)
		{
			mPort = serverPort;
			mListener = new UdpClient(mPort);
		}

		public void Run()
		{
			running = true;
			IPEndPoint messageSourceEndpoint = new IPEndPoint(IPAddress.Any, 0);

			Console.WriteLine("Running echo server at localhost:{0}", mPort);

			while (running == true) {
				if (mListener.Available > 0) {
					byte[] messageBuffer = mListener.Receive(ref messageSourceEndpoint);
					string message = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);
					Console.WriteLine("[Client] {0}", message);
					SendMessage(message, messageSourceEndpoint);
				}
			}
		}

		protected void SendMessage(string messageText, IPEndPoint destination)
		{
			byte[] encodedMessage = Encoding.ASCII.GetBytes(messageText);
			mListener.Client.SendTo(encodedMessage, destination);
		}

		public void Stop()
		{
			running = false;
			mListener.Close();
		}
	}
}

