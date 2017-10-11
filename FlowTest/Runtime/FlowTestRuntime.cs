using System.Net;
using System.IO;
using System.Collections.Generic;
using System;

namespace FlowTest
{
	public class FlowTestRuntime
	{
        private FlowTestWeavingOrchestration weavingHandler = new FlowTestWeavingOrchestration();
        private FlowTestEventAggregator eventHandler = new FlowTestEventAggregator();
        private FlowTestStartupExecutables executableHandler = new FlowTestStartupExecutables();

        public FlowTestWeavingOrchestration Weaver { get { return weavingHandler; } }
        public FlowTestEventAggregator Events { get { return eventHandler; } }
        public  FlowTestStartupExecutables Execution { get { return executableHandler; } }

        public FlowTestRuntime () {}

        #region Instrumentation and Weaving

        public void AddPointOfInterest(FlowTestPointOfInterest poi)
        {
            poi.setRuntime (this);
            weavingHandler.weavePointOfInterest(poi);
        }

        public void Write()
        {
            weavingHandler.Write();
        }

        #endregion

        #region Execution

		public void addExecutableToFlowTestStartup(
			string pathToExecutable,
			string arguments,
            int nSecondsOnLaunch, 
			string workingDirectory = null
		)
		{
            executableHandler.addExecutableToLaunch(
                path: pathToExecutable,
                args: arguments,
                nSecondsDelay: nSecondsOnLaunch,
                workingDirectory: workingDirectory
            );
		}

        public void Start()
        {
            eventHandler.Start ();
            executableHandler.Start();
        }

        public void Stop()
        {
            eventHandler.Stop();
            executableHandler.Stop();
        }

        public List<string> getExecutableLog(string exePath)
        {
            return executableHandler.getStdOutEventsFromProcess(exePath);
        }

        #endregion

        #region Testing, Regressions, Asserts

        #endregion
	}
}

