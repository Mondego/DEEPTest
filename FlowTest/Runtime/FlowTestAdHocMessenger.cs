using System;
using System.Net.Sockets;
using System.Text;

namespace FlowTest
{
	public class FlowTestAdHocMessenger
	{
		// This whole thing is basically a hotfix
		public static void SendBlobToFlowTestDriver(string serializedEvent)
		{
			int defaultFlowTestDriverPort = 60011;
			TcpClient tcpc = new TcpClient("127.0.0.1", defaultFlowTestDriverPort);
			NetworkStream ns = tcpc.GetStream();
			byte[] messageData = Encoding.ASCII.GetBytes(serializedEvent);
			ns.Write(messageData, 0, messageData.Length);
			ns.Close();
			tcpc.Close();
		}
	}
}

