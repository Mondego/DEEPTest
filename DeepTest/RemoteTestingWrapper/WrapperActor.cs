using System;

using Akka.Actor;

using RemoteAssertionMessages;

namespace RemoteTestingWrapper
{
    public class WrapperActor : TypedActor,
    IHandle<AssertionResultMessage>
    //, ILogReceive
    {
        private readonly ActorSelection _assertionsHome = 
            Context.ActorSelection(
                "akka.tcp://DeepTestDriverSystem@127.0.0.1:60713/user/DTInteralTestDriver"
            );

        public void Handle(AssertionResultMessage message)
        {
            message.AboutRemoteWrapper = "Weaving.WrapperActor [about]";
            message.AssertionContext = "Weaving.WrapperActor [context]";
            message.AssertionResult = "Weaving.WrapperActor [result]";

            _assertionsHome.Tell(message, Self);
        }

        public void Handle(Terminated message)
        {
            Console.Write("AWAY.Handle.Terminated Homebase died");
        }
    }
}