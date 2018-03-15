using System;

namespace TestDriverAPI
{
    public class InstrumentationHelper
    {
        public InstrumentationHelper()
        {
        }

        public InstrumentationHelper At(InstrumentationPoint ip)
        {
            return this;
        }

        public InstrumentationHelper AtEntry(InstrumentationPoint ip)
        {
            return this;
        }

        public InstrumentationHelper AtExit(InstrumentationPoint ip)
        {
            return this;
        }

        public InstrumentationHelper Until(InstrumentationPoint ip)
        {
            return this;
        }

        public InstrumentationHelper UntilEntry(InstrumentationPoint ip)
        {
            return this;
        }

        public InstrumentationHelper UntilExit(InstrumentationPoint ip)
        {
            return this;
        }
    }
}

