using System;
using System.Collections.Generic;
using InternalTestDriver;
using System.Threading.Tasks;
using System.Threading;

namespace DeepTestFramework
{
    public class DTRuntime
    {
        public WeavingHandler weavingHandler;
        private Dictionary<string, DTNodeDefinition> executionDefinitions; 
        private AdHocInternalDriver tempDriver = new AdHocInternalDriver();

        public DTRuntime()
        {
            executionDefinitions = new Dictionary<string, DTNodeDefinition>();
            weavingHandler = new WeavingHandler(metadata: tempDriver.getDriverPort());
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

        public async Task<bool> remoteAssertionTookAtMost(double expectedMax)
        {
            double actual = 9000.0;

            if (actual <= expectedMax)
            {
                return true;
            }

            else { 
                return false; 
            }
        }

        public void StartDriver()
        {
            Thread.Sleep(3000);
        }

        public void StopAll()
        {
            tempDriver.Disactivate();

            foreach (DTNodeDefinition v in executionDefinitions.Values) {
                v.Stop();
            }
        }
    }
}

