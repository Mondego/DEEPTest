using System;

namespace TestDriverAPI
{
    public class DeepTestHandler
    {
        public InstrumentationAPI Instrumentation { get; }
        public TestExecutionAPI Deployment { get; }

        public DeepTestHandler()
        {
            Instrumentation = new InstrumentationAPI();
            Deployment = new TestExecutionAPI();
        }
    }
}

