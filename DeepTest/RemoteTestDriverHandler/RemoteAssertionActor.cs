using System;

using Akka.Actor;

using RemoteAssertionMessages;

namespace RemoteTestDriverHandler
{
    public class RemoteAssertionActor : TypedActor,
    IHandle<ConnectionRequest>,
    IHandle<ConnectionResponse>,
    IHandle<AssertionResultMessage>,
    ILogReceive
    {
        // TODO this is hardcoded at the moment
        private readonly ActorSelection _homebase = Context.ActorSelection("akka.tcp://DeepTestDriverSystem@127.0.0.1:60713/user/DTInteralTestDriver");
        private string _metadata = "mRemoteAssertionInfo";

        public void Handle(ConnectionRequest message)
        {
            message.AboutRemoteWrapper = this._metadata;

            Console.WriteLine("AWAY.Handle.ConnectionRequest [{0}] {1}", 
                Self.Path.ToString(),
                message.AboutRemoteWrapper
            );

            _homebase.Tell(message, Self);
        }

        public void Handle(ConnectionResponse message)
        {
            Console.WriteLine("AWAY.Handle.ConnectionResponse [{0}] {1}",
                Sender.Path.ToString(),
                message.Message
            );
        }

        public void Handle(AssertionResultMessage message)
        {
            message.AboutRemoteWrapper = this._metadata;
            message.AssertionContext = "AWAY.AssertionResultMessage --- [context]";
            message.AssertionResult = "AWAY.AssertionResultMessage --- [result]";

            Console.WriteLine("ROAMER.Handle.RoamerStatusMessage reporting [{0}] {1} {2} {3}",
                Self.Path.ToString(),
                message.AboutRemoteWrapper,
                message.AssertionContext,
                message.AssertionResult
            );

            _homebase.Tell(message, Self);
        }

        public void Handle(Terminated message)
        {
            Console.Write("AWAY.Handle.Terminated Homebase died");
        }
    }
}
