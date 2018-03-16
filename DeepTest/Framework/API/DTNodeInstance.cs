using System;


namespace DeepTestFramework
{
    public class DTNodeInstance
    {
        public SystemProcessWithInput mProcess { get; }
        private string path;
        private string args;
        private int delay;
        private string directory;

        public DTNodeInstance(
            string executionPath,
            string arguments,
            int nSecondsDelay = 0,
            string workingDirectory = null
        )
        {
            path = executionPath;
            args = arguments;
            delay = nSecondsDelay;
            directory = workingDirectory;

            mProcess = new SystemProcessWithInput(
                targetPath: path,
                arguments: args,
                workingdir: directory
            );

            mProcess.Start(nSecondsDelay);
        }
    }
}

