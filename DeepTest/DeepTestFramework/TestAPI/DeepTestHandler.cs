using System;

namespace DeepTestFramework
{
    public class DeepTestHandler
    {
        public InstrumentationAPI Instrumentation { get; }
        public SystemUnderTestDeploymentAPI Deployment { get; }

        public DeepTestHandler()
        {
            Instrumentation = new InstrumentationAPI();
            Deployment = new SystemUnderTestDeploymentAPI();
        }
    }
}

