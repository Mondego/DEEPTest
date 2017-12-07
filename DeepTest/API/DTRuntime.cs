using System;
using System.Collections.Generic;

namespace DeepTest
{
    public class DTRuntime
    {
        private Dictionary <string, DTNode> sutExecution = new Dictionary<string, DTNode>();

        public DTRuntime()
        {
        }

        public DTNode addSystemUnderTest(string path)
        {
            DTNode sut = new DTNode(path);
            sutExecution.Add(path, sut);
            return sut;
        }

        public void StopAll()
        {
            foreach (DTNode v in sutExecution.Values) {
                v.Stop();
            }
        }
    }
}

