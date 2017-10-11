using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FlowTest
{
    public class FlowTestStartupExecutables
    {
        private OrderedDictionary mapOfExecutablePathsToLaunchConfigurations = new OrderedDictionary();

        public FlowTestStartupExecutables () 
        {
            
        }

        public void addExecutableToLaunch(
            string path,
            string args,
            int nSecondsDelay,
            string workingDirectory = null
        )
        {
            mapOfExecutablePathsToLaunchConfigurations.Add(
                key: path,
                value: new LaunchConfiguration(
                    executionPath: path,
                    arguments: args,
                    nSecondsForStartup: nSecondsDelay,
                    workingDir: workingDirectory
                )
            );
        }

        public List<string> getStdOutEventsFromProcess(string key)
        {
            return ((LaunchConfiguration)mapOfExecutablePathsToLaunchConfigurations[key]).Log;
        }

        public void Start()
        {
            foreach (DictionaryEntry exe in mapOfExecutablePathsToLaunchConfigurations) {
                ((LaunchConfiguration)exe.Value).Start();
            }
        }

        public void Stop()
        {
            for (int i = mapOfExecutablePathsToLaunchConfigurations.Count - 1; i >= 0; i--)
            {
                ((LaunchConfiguration)mapOfExecutablePathsToLaunchConfigurations[i]).Stop();
            }
        }
    }
}

