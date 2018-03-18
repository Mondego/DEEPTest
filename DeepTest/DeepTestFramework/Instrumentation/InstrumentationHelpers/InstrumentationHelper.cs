using System;
using Mono.Cecil;

namespace DeepTestFramework
{
    public class InstrumentationHelper
    {
        private InstrumentationAPI instrumentationAPI;

        public InstrumentationHelper(InstrumentationAPI instrumentation)
        {
            instrumentationAPI = instrumentation;
        }

        public InstrumentationHelper AtEntry(InstrumentationPoint ip)
        {
            //InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodEntry(ip, ???);

            return _Save(ip);
        }

        public InstrumentationHelper AtExit(InstrumentationPoint ip)
        {
            //InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodExit(ip, ???);

            return _Save(ip);
        }

        public InstrumentationHelper UntilEntry(InstrumentationPoint ip)
        {
            //InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodEntry(ip, ???);

            return _Save(ip);
        }

        public InstrumentationHelper UntilExit(InstrumentationPoint ip)
        {
            //InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodExit(ip, ???);

            return _Save(ip);
        }

        private InstrumentationHelper _Save(InstrumentationPoint ip)
        {
            instrumentationAPI.updateAssembly(ip);
            instrumentationAPI.writeAssembly(ip);

            return this;
        }
    }
}

