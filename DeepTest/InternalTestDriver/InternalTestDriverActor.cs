using System;
using System.Collections.Generic;

using Akka.Actor;

using RemoteAssertionMessages;

namespace InternalTestDriver
{
    public class InternalTestDriverActor : TypedActor,
    IHandle<ConnectionRequest>,
    IHandle<AssertionResultMessage>,
    ILogReceive
    {
        private readonly HashSet<IActorRef> _remoteTestWrappers = new HashSet<IActorRef>();

        public void Handle(ConnectionRequest message)
        {
            Console.WriteLine("ITD.Handle.ConnectionRequest [{0}] {1}", 
                Sender.Path.ToString(),
                message.AboutRemoteWrapper 
            );

            _remoteTestWrappers.Add(this.Sender);
            Sender.Tell(new ConnectionResponse
                {
                    Message = "Connected",
                }, Self);
        }

        public void Handle(AssertionResultMessage message)
        {
            Console.WriteLine("HOME.Handle.RoamerStatusMessage [{0}] {1} {2} {3}",
                Sender.Path.ToString(),
                message.AboutRemoteWrapper, 
                message.AssertionContext,
                message.AssertionResult
            );

            // Placeholder for chatting with DTR
        }
    }
}

