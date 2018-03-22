using System;

namespace RemoteAssertionMessages
{
    public class InstrumentationPointExchangeMessage
    {
        public string instrumentationPointName { get; set; }
        public string instrumentationPointType { get; set; }
        public object value { get; set; }
    }
}
