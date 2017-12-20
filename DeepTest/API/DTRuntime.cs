using System;
using System.Collections.Generic;

namespace DeepTest
{
    public class DTRuntime
    {
        private WeavingHandler weavingHandler = new WeavingHandler();
        private Dictionary<string, DTNodeDefinition> executionDefinitions;

        public WeavingHandler Instrumentation
        {
            get {
                return weavingHandler;
            }
        }

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
                Console.WriteLine("DTNodeDefinition.addSystemUnderTest(path) caught exception {0} {1}", 
                    e.ToString(), 
                    e.Message); 
            }

            return null;
        }

        public void StopAll()
        {
            foreach (DTNodeDefinition v in executionDefinitions.Values) {
                v.Stop();
            }
        }
    }
}

