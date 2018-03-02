using System;

namespace RemoteAssertionMessages
{
    public class ConnectionRequest
    {
        public string AboutRemoteWrapper { get; set; }
    }

    public class ConnectionResponse
    {
        public string Message { get; set; }
    }

    public class AssertionResultMessage
    {
        public string AboutRemoteWrapper { get; set; }
        public string AssertionResult { get; set; }
        public string AssertionContext { get; set; } 
    }
}