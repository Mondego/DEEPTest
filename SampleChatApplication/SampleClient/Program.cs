using System;

namespace SampleClient
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			int chatServerPort = (int)Int64.Parse (args [0]);

			ChatClient chat = new ChatClient (chatServerPort);
			chat.Run();
		}
	}
}
