using System;
using System.Collections.Generic;
using System.Threading;

namespace FlowTest
{
    public class LaunchConfiguration
    {
        public string launchFilePath { get; }
        public int nSecondsWarmup { get; }
        public ProcessExecutionWithRedirectedIO process { get; }
        public List<string> Log { get { return process.Log; } }

        public LaunchConfiguration(
            string executionPath, string arguments, int nSecondsForStartup = 0, string workingDir = null
        )
        {
            launchFilePath = executionPath;
            nSecondsWarmup = nSecondsForStartup;
            process = new ProcessExecutionWithRedirectedIO (executionPath, arguments, workingDir);
        }

        public void Start()
        {
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

