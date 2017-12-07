using System;

namespace Test.Example.EchoChatServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string echoServerHost = args[0];
            int echoServerPort = (int)Int64.Parse (args [1]);

            EchoChatServer server = new EchoChatServer(echoServerHost, echoServerPort);
            server.Start();
        }
    }
}
