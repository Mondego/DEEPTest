using System;

namespace DeepTest
{
    public class DTNode
    {
        private string readPath;
        DTProcess mProcess = null;

        public DTNode(string assemblyPath)
        {
            readPath = assemblyPath;
        }

        public override string ToString()
        {
            return String.Format("DTNode {0}", readPath);
        }

        /// <summary>
        /// Execute the specified executablePath, argumentString, nSecondsDelay and workingDirectory.
        /// </summary>
        /// <param name="executionPath">Path of executable if written elsewhere</param></param>
        /// <param name="argumentString">Argument string.</param>
        /// <param name="nSecondsDelay">N seconds delay.</param>
        /// <param name="workingDirectory">Working directory.</param>
        public DTProcess Start(
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

            mProcess = new DTProcess(
                targetPath: runPath,
                arguments: argumentString,
                workingdir: workingDirectory
            );
            
            mProcess.Start(nSecondsDelay);

            return mProcess;
        }

        public void Stop()
        {
            mProcess.Stop();
        }
    }
}

