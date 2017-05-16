using System;

namespace SampleServer
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			int echoServerPort = (int)Int64.Parse (args [0]);

			ChatServer server = new ChatServer(echoServerPort);
			server.Run();
		}
	}
}
