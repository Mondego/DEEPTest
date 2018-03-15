using System;

namespace TestDriverAPI
{
    public class MeasurementHandler
    {
        public MeasurementHandler() {}

        public InstrumentationHelper WithStopWatch()
        {
            return new InstrumentationHelper();
        }
    }
}

