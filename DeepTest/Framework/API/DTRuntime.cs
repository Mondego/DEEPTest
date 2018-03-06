using System;
using System.Collections.Generic;
using InternalTestDriver;

namespace DeepTestFramework
{
    public class DTRuntime
    {
        private WeavingHandler weavingHandler = new WeavingHandler();
        private Dictionary<string, DTNodeDefinition> executionDefinitions = new Dictionary<string, DTNodeDefinition>();
        //private DTProcess networkedTestDriver;
        private InternalTestDriverMain mInternalDriver;

        public WeavingHandler Instrumentation
        {
            get {
                return weavingHandler;
            }
        }

        public DTRuntime()
        {
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

        public void StartDriver(string workingDirectory)
        {
            mInternalDriver = new InternalTestDriverMain();
        }

        public void StopAll()
        {
            /*if (networkedTestDriver != null && !networkedTestDriver.p.HasExited) {
                networkedTestDriver.Stop();
            }*/

            foreach (DTNodeDefinition v in executionDefinitions.Values) {
                v.Stop();
            }
        }
    }
}

