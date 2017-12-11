using System;
using System.Collections.Generic;

namespace DeepTest
{
    public class DTRuntime
    {
        private WeavingHandler weavingHandler;
        private Dictionary<string, DTNodeDefinition> executionDefinitions;

        public DTRuntime()
        {
            weavingHandler = new WeavingHandler();
            executionDefinitions = new Dictionary<string, DTNodeDefinition>();
        }

        public DTNodeDefinition addSystemUnderTest(string path)
        {
            try
            {
                weavingHandler.ReadAssembly(path);
                DTNodeDefinition sut = new DTNodeDefinition(path);
                executionDefinitions.Add(path, sut);

                return sut;
            }

            catch (Exception e) {

            }

            return null;
        }

        public void StopAll()
        {
            foreach (DTNodeDefinition v in executionDefinitions.Values) {
                v.Stop();
            }
        }

        public WeavePoint AddWeavePoint(DTNodeDefinition component, string weaveIntoType, string weaveInMethod)
        {
            return weavingHandler.AddWeavePointToNode(component, weaveIntoType, weaveInMethod);
        }

        public void Write(DTNodeDefinition componentToWrite)
        {
            weavingHandler.Write(componentToWrite);
        }
    }
}

