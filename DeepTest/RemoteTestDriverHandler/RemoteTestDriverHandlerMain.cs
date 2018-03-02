using System;
using Akka.Actor;
using Akka.Configuration;
using RemoteAssertionMessages;

namespace RemoteTestDriverHandler
{
    class RemoteTestDriverHandlerMain
    {
        public static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = remote
    }
    remote {
        dot-netty.tcp {
            port = 0
            hostname = localhost
        }
    }
}
");
         
            using (var system = ActorSystem.Create("AdHocRoamingSystem", config)) {
                var mRoamer = system.ActorOf(Props.Create<RemoteAssertionActor>());

                // mRoamer.Tell(new ConnectionRequest() { });
                // mRoamer.Tell(new AssertionResultMessage() {});
                system.Terminate().Wait();
            }
        }
    }
}
