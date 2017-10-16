using System.Net;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;

namespace FlowTest
{
	public class FlowTestRuntime
	{
        private FlowTestModuleOrchestration instrumentationHandler = new FlowTestModuleOrchestration();
        private FlowTestEventAggregator eventHandler = new FlowTestEventAggregator();

        public FlowTestModuleOrchestration Instrumentation { get { return instrumentationHandler; } }
        public FlowTestEventAggregator Events { get { return eventHandler; } }

        public FlowTestRuntime () {}

        #region Execution

        /// <summary>
        /// Starts the event logger, and then starts in order any executables to run.
        /// </summary>
        public void start()
        {
            eventHandler.Start ();
            instrumentationHandler.StartLaunchSequence();
        }

        /// <summary>
        /// Stops the event logger and any ongoing exes
        /// </summary>
        public void stopAndCleanup()
        {
            eventHandler.Stop();
            instrumentationHandler.StopLaunchSequence();

            foreach (string sourcePath in instrumentationHandler.mapOfReadPathsToWritePaths.Keys) {
                string destinationPath = instrumentationHandler.mapOfReadPathsToWritePaths[sourcePath];
                if (sourcePath != destinationPath) {
                    File.Delete(destinationPath);
                }
            }
        }

        #endregion

        #region Testing, Regressions, Asserts

        /// <summary>
        /// Gets all stdout data from running exe
        /// </summary>
        /// <returns>List of stdout data from running exe</returns>
        /// <param name="exePath>the original exe pat</param>
        public List<string> getExecutableLog(string exePath)
        {
            return instrumentationHandler.getStdOutEventsFromLaunchConfig(exePath);
        }

        #endregion
	}
}

