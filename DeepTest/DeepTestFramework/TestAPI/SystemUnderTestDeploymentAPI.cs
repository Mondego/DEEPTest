using System;

namespace DeepTestFramework
{
    public class SystemUnderTestDeploymentAPI
    {
        public SystemUnderTestDeploymentAPI()
        {
        }

        public SystemProcessWithInput ExecutePath(string path)
        {
            return ExecuteWithArguments(path, "");
        }

        public SystemProcessWithInput ExecuteWithArguments(string path, string arguments)
        {
            return new SystemProcessWithInput(path, arguments);
        }
    }
}

