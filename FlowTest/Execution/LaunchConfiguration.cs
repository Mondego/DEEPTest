using System;
using System.Collections.Generic;
using System.Threading;

namespace FlowTest
{
    public class LaunchConfiguration
    {
        public string launchOriginalPath { get; }
        public string launchFilePath { get; }
        public int nSecondsWarmup { get; }
        public string args { get; }
        public string workingDirectory { get; }

        private ProcessExecutionWithRedirectedIO process;
        public List<string> Log { get { return process.Log; } }

        public LaunchConfiguration(
            string originalPath,
            string executionPath = null,
            string arguments = "",
            int nSecondsForStartup = 0, 
            string workingDir = null
        )
        {
            launchOriginalPath = originalPath;

            if (executionPath != null) {
                launchFilePath = executionPath;
            } else {
                launchFilePath = originalPath;
            }

            args = arguments;
            nSecondsWarmup = nSecondsForStartup;
            workingDirectory = workingDir;
        }

        public void Start()
        {
            process = new ProcessExecutionWithRedirectedIO (launchFilePath, args, workingDirectory);
            process.Start();
            Thread.Sleep(nSecondsWarmup * 1000);
        }

        public void Stop()
        {
            if (process.PID != -1) {
                process.Stop();
            }
        }
    }
}

