using System;
using System.Collections.Generic;

namespace DeepTest
{
    public class DTRuntime
    {
        public static Dictionary<int, AssertionResult> results;
        private WeavingHandler weavingHandler;
        private Dictionary<string, DTNodeDefinition> executionDefinitions;
        public WeavingHandler Instrumentation
        {
            get {
                return weavingHandler;
            }
        }

        public DTRuntime()
        {
            executionDefinitions = new Dictionary<string, DTNodeDefinition>();
            weavingHandler = new WeavingHandler();
            results = new Dictionary<int, AssertionResult>();
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

        // Maybe these need their own library later
        public static void updateAssertionResultEntry(int key, string value)
        {
            Console.WriteLine("DTRuntime.updateAssertionResultEntry {0}->{1}", key, value);
            results.Add(key, new AssertionResult(key, value));
        }

    }
}

