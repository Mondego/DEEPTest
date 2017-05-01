using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Text;

namespace SampleClient
{
	public class ChatClient
	{
		protected int mServerPort;
		protected bool running = false;
		protected UdpClient mClient;

		public ChatClient (int chatServerPort)
		{
			mServerPort = chatServerPort;
			mClient = new UdpClient(0);
		}

		public void Run()
		{
			running = true;
			IPEndPoint MessageListeningEndpoint = new IPEndPoint(IPAddress.Any, 0);

			Console.WriteLine("Running chat client at localhost:{0}", 
				((IPEndPoint)mClient.Client.LocalEndPoint).Port);

			Task.Run(() => {
				while (running == true)
				{
					string message = Console.ReadLine();
					Console.WriteLine("[Client] {0}", message);
					SendMessage(message);
				}
			});

			while (running) {
				if (mClient.Available > 0) {
					byte[] nextMessageBuffer = mClient.Receive(ref MessageListeningEndpoint);
					string nextMessageText = Encoding.ASCII.GetString(nextMessageBuffer, 0, nextMessageBuffer.Length);
					ReceivedMessage(nextMessageText);
				}
			}
		}

		protected void SendMessage(string message)
		{
			IPEndPoint destination = new IPEndPoint(IPAddress.Any, mServerPort);
			byte[] encodedMessage = Encoding.ASCII.GetBytes(message);
			mClient.Client.SendTo(encodedMessage, destination);
		}

		protected void ReceivedMessage(string message)
		{
			Console.WriteLine("[From Server] {0}", message);
		}

		public void Stop()
		{
			running = false;
			mClient.Close();
		}
	}
}

