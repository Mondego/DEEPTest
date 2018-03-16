using System;

namespace DeepTestFramework
{
    public class SnapshotHandler
    {
        private InstrumentationAPI instrumentationSource;

        public SnapshotHandler(InstrumentationAPI i) {
            instrumentationSource = i;
        }

        public InstrumentationHelper ValueOf(string field)
        {
            return new InstrumentationHelper(instrumentationSource);
        }
    }
}

