using System;
using System.Diagnostics;

using RemoteAssertionMessages;

using Akka.Actor;
using Akka.Configuration;

namespace RemoteTestingWrapper
{
    public sealed class RemoteSafeAssertionsSingleton
    {
        private ActorSystem mSystem; 

        private RemoteSafeAssertionsSingleton()
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
            mSystem = ActorSystem.Create("WovenRoamingSystem", config);

            // mRoamer.Tell(new AssertionResultMessage() {});
        }

        public static RemoteSafeAssertionsSingleton Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly RemoteSafeAssertionsSingleton instance = new RemoteSafeAssertionsSingleton();
        }

        // Custom methods here
        public void Message(Stopwatch s)
        {
            Console.WriteLine("called the singleton message " + s.ElapsedMilliseconds / 1000.0 + " s");
        
            AssertionResultMessage s2 = new AssertionResultMessage();
            s2.AboutRemoteWrapper = "about";
            s2.AssertionResult = "result";
            s2.AssertionContext = "context";
        }
    }
}
