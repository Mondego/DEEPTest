using System;
using System.Diagnostics;

namespace RemoteTestingWrapper
{
    public class WeavingAssertionHandler
    {
        public static void HelloWorld()
        {
            
        }

        public static void stopwatchResultHook(ref Stopwatch s)
        {
            Console.WriteLine("StopWatchResultEntry === " + s.ElapsedMilliseconds/1000.0 + " s");
            HelloWorld();
        }
    }
}

