using System;
using System.Collections.Generic;

namespace Framework
{
    public class DTNodeDefinition
    {
        public string readPath { get; }
        private List<DTNodeInstance> nodeInstances;

        public DTNodeDefinition(string assemblyPath)
        {
            readPath = assemblyPath;
            nodeInstances = new List<DTNodeInstance>();
        }

        public override string ToString()
        {
            return String.Format("DTNodeDefinition {0}", readPath);
        }

        /// <summary>
        /// Execute the specified executablePath, argumentString, nSecondsDelay and workingDirectory.
        /// </summary>
        /// <param name="executionPath">Path of executable if written elsewhere</param></param>
        /// <param name="argumentString">Argument string.</param>
        /// <param name="nSecondsDelay">N seconds delay.</param>
        /// <param name="workingDirectory">Working directory.</param>
        public void StartInstance(
            string externalPath = "",
            string argumentString = "",
            int nSecondsDelay = 0,
            string workingDirectory = null
        )
        {
            string runPath = readPath;

            if (externalPath != "") {
                runPath = externalPath;    
            }

            DTNodeInstance nInstance = 
                new DTNodeInstance(
                    runPath,
                    argumentString,
                    nSecondsDelay,
                    workingDirectory
                );

            nodeInstances.Add(nInstance);
        }

        public void Stop()
        {
            foreach (DTNodeInstance dtni in nodeInstances) {
                dtni.mProcess.Stop();
            }
        }
    }
}

