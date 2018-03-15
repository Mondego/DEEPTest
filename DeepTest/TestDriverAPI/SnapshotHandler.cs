using System;

namespace TestDriverAPI
{
    public class SnapshotHandler
    {
        public SnapshotHandler() {}

        public InstrumentationHelper ValueOf(string field)
        {
            return new InstrumentationHelper();
        }
    }
}

