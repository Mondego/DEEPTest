using System;

namespace DeepTestFramework
{
    public class SnapshotHandler
    {
        private InstrumentationAPI instrumentationSource;

        public SnapshotHandler(InstrumentationAPI i) {
            instrumentationSource = i;
        }

        public InstrumentationHelper ValueOfField(string fieldName)
        {
            return new SnapshotLocalValueHelper(instrumentationSource, fieldName);
        }
    }
}

