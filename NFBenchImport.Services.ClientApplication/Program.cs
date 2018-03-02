using System;

namespace NFBenchImport.Services.ClientApplication
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            { 
                string hostname = args[0];
                int port = (int)Int64.Parse(args[1]);
                int id = (int)Int64.Parse(args[2]);

                ClientApplication chatClient = new ClientApplication(hostname, port, id);
                chatClient.start();
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Hello World!");
        }
    }
}
