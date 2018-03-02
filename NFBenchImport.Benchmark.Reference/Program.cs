using System;

namespace NFBenchImport.Benchmark.Reference
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                string benchmarkName = args[0];
                string hostname = args[1];
                int port = (int)Int64.Parse (args [2]);

                ReferenceApplicationServer referenceApp = new ReferenceApplicationServer(hostname, port);
                referenceApp.start();
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
