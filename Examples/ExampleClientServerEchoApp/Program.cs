using System;

namespace ExampleClientServerEchoApp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args[0] == "server") {
                EchoServer server = new EchoServer((int)Int64.Parse(args[1]));
                server.Start();
            } else if (args[0] == "client") {
                EchoClient client = new EchoClient((int)Int64.Parse(args[1]));
                client.Start();
            }
        }
    }
}
