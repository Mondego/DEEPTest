using System;

using Akka.Configuration;
using Akka.Actor;

namespace InternalTestDriver
{
    public sealed class InternalTestDriverMain
    {
        public static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor 
    {
        provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    
    remote {
        dot-netty.tcp {
            port = 60713
            hostname = 127.0.0.1
        }
    }
}
");
            try 
            {
                using (var system = ActorSystem.Create("DeepTestDriverSystem", config))
                {
                    system.ActorOf<InternalTestDriverActor>("DTInteralTestDriver");

                    Console.ReadLine();
                }
            }

            catch (Exception e) {
                Console.WriteLine("InternalTestDriverMain caught " + e.Message);
            }
        }
    }
}

