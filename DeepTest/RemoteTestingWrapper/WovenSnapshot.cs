using System;
using System.Collections.Generic;

namespace RemoteTestingWrapper
{
    public class WovenSnapshot
    {
        public object Value { get; private set; }
        public string Name { get; private set; } 

        public WovenSnapshot(string ipName, object value)
        {
            Console.WriteLine("Creating wovensnapshot for ip {0} --- value {1}", ipName, value.ToString());
            Name = ipName;
            Value = value;
            StandaloneInstrumentationMessageHandler.Instance.Register(this);
        }

        public WovenSnapshot Update(object val)
        {
            Value = val;

            return this;
        }
    }
}
