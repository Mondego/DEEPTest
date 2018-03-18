using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace DeepTestFramework
{
    public class DTRuntime
    {
        public WeavingHandler weavingHandler;
        private Dictionary<string, DTNodeDefinition> executionDefinitions; 

        public DTRuntime()
        {
            executionDefinitions = new Dictionary<string, DTNodeDefinition>();
            weavingHandler = new WeavingHandler(0);
        }

        public WeavingHandler getInstrumentation()
        {
            return weavingHandler;
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

        public void StartDriver()
        {
            Thread.Sleep(3000);
        }

        public void StopAll()
        {
            foreach (DTNodeDefinition v in executionDefinitions.Values) {
                v.Stop();
            }
        }
    }
}

