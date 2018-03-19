using System;

namespace DeepTestFramework
{
    public class DelayHandler
    {
        private InstrumentationAPI instrumentationSource;

        public DelayHandler(InstrumentationAPI i)
        {
            instrumentationSource = i;
        }

        public InstrumentationHelper AddSecondsOfSleep(int nSeconds)
        {
            return new SleepHelper(instrumentationSource, nSeconds);
        }
    }
}

