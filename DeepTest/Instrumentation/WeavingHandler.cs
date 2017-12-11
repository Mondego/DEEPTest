using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace DeepTest
{
    public class WeavingHandler
    {
        private Dictionary<string, AssemblyDefinition> mWeaves;

        public WeavingHandler()
        {
            mWeaves = new Dictionary<string, AssemblyDefinition>();
        }
            
        public WeavePoint AddWeavePointToNode(
            DTNodeDefinition target,
            string nameOfWeavePointType,
            string nameOfWeavePointMethod
        )
        {
            try
            {
                WeavePoint wp = new WeavePoint(
                    target.readPath,
                    nameOfWeavePointType,
                    nameOfWeavePointMethod
                );

                return wp;
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.AddWeavePointToNode caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }

            return null;
        }

        public void ReadAssembly(string path)
        {
            try
            {
                if (!mWeaves.ContainsKey(path))
                {
                    AssemblyDefinition m = AssemblyDefinition.ReadAssembly(path);
                    mWeaves.Add(path, m);
                }
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.AddWeavePointToNode caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }

        public void Write(DTNodeDefinition target, string alternateWritePath = "")
        {
            try
            {
                string writePath = target.readPath;
                if (alternateWritePath != "")
                {
                    writePath = alternateWritePath;
                }

                AssemblyDefinition assemblyToWrite = mWeaves[target.readPath];
                assemblyToWrite.Write(writePath);
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.Write() caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }
    }
}

