using System;
using System.Net.Sockets;
using System.Text;

namespace FlowTest
{
	// Version 4 of Jon Skeet's C# in Depth threadsafe Singletons guide
	public sealed class FlowTestProxySingleton
	{
		private static readonly FlowTestProxySingleton instance = new FlowTestProxySingleton();

		// Explicit static constructor to tell C# compiler
		// not to mark type as beforefieldinit
		static FlowTestProxySingleton () {}
		private FlowTestProxySingleton () {
			Console.WriteLine("Constructor for FlowTestProxySingleton");
		}

		public static FlowTestProxySingleton Instance { get { return instance; } }

		public void Message(string message)
		{
			int defaultFlowTestDriverPort = 60011;
			TcpClient tcpc = new TcpClient("127.0.0.1", defaultFlowTestDriverPort);
			NetworkStream ns = tcpc.GetStream();
			byte[] messageData = Encoding.ASCII.GetBytes(message);
			ns.Write(messageData, 0, messageData.Length);
			ns.Close();
			tcpc.Close();
		}
	}
}

