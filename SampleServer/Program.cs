using System;

// Maybe I need this
using FlowTestAPI;

namespace SampleServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			int echoServerPort = (int)Int64.Parse (args [0]);

			EchoServer server = new EchoServer(echoServerPort);
			server.Run();
		}
	}
}
