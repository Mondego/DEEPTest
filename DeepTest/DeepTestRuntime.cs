using System.Net;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;
using Mono.Cecil;

namespace DeepTest
{
	public class DeepTestRuntime
	{
        public DeepTestModuleInstrumentation instrumentation { get; }

        public DeepTestRuntime () {
            instrumentation = new DeepTestModuleInstrumentation();
        }

        #region Execution
        /// <summary>
        /// Execute the specified executablePath, argumentString, nSecondsDelay and workingDirectory.
        /// </summary>
        /// <param name="executablePath">Executable path.</param>
        /// <param name="argumentString">Argument string.</param>
        /// <param name="nSecondsDelay">N seconds delay.</param>
        /// <param name="workingDirectory">Working directory.</param>
        public DTProcess Execute(
            string executablePath,
            string argumentString = "",
            int nSecondsDelay = 0,
            string workingDirectory = null
        )
        {
            DTProcess p = 
                new DTProcess(
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

