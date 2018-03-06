using System;
using System.Diagnostics;

using RemoteAssertionMessages;

using Akka.Actor;
using Akka.Configuration;
using System.Collections.Generic;

namespace RemoteTestingWrapper
{
    public sealed class RemoteSafeAssertionsSingleton
    {
        private ActorSystem mSystem;
        private Dictionary<string, string> mResults;

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

            var sendStopwatchActor = mSystem.ActorOf(Props.Create<WrapperActor>());

            sendStopwatchActor.Tell(new AssertionResultMessage() {
                AboutRemoteWrapper = "WeavingAssertionHandler",
                AssertionContext = "Stopwatch",
                AssertionResult = s.ElapsedMilliseconds.ToString()
            });
        }
    }
}
