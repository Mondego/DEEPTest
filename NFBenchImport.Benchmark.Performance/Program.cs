using System;

namespace NFBenchImport.Benchmark.Performance
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

                PerformanceBugApplicationServer performanceApp = new PerformanceBugApplicationServer(hostname, port);
                performanceApp.start();
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
