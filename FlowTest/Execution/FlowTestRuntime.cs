using System.Net;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;
using Mono.Cecil;

namespace FlowTest
{
	public class FlowTestRuntime
	{
        public FlowTestModuleInstrumentation instrumentation { get; }

        public FlowTestRuntime () {
            instrumentation = new FlowTestModuleInstrumentation();
        }

        #region Execution
        /// <summary>
        /// Execute the specified executablePath, argumentString, nSecondsDelay and workingDirectory.
        /// </summary>
        /// <param name="executablePath">Executable path.</param>
        /// <param name="argumentString">Argument string.</param>
        /// <param name="nSecondsDelay">N seconds delay.</param>
        /// <param name="workingDirectory">Working directory.</param>
        public FTProcess Execute(
            string executablePath,
            string argumentString = "",
            int nSecondsDelay = 0,
            string workingDirectory = null
        )
        {
            FTProcess p = 
                new FTProcess(
                    targetPath: executablePath,
                    arguments: argumentString,
                    workingdir: workingDirectory
                );
            p.Start(nSecondsDelay);

            return p;
        }

        /// <summary>
        /// Stub for now, stops any internal runtime threads.
        /// </summary>
        public void Stop()
        {
        }

        #endregion

        #region Testing, Regressions, Asserts

        #endregion
	}
}

