using System;

namespace DeepTestFramework
{
    public class MeasurementHandler
    {
        private InstrumentationAPI instrumentationSource;

        public MeasurementHandler(InstrumentationAPI i) 
        {
            instrumentationSource = i;
        }

        public InstrumentationHelper WithStopWatch()
        {
            return new InstrumentationHelper(instrumentationSource);
        }
    }
}

