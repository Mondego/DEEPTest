using System;
using System.Diagnostics;

namespace RemoteTestingWrapper
{
    public class WeavingAssertionHandler
    {
        public static void deepTestConnectionSkeleton(int key, string value)
        {
            Console.WriteLine("PLACEHOLDER");
            Console.WriteLine(key);
            Console.WriteLine(value);
            //DTRuntime.updateAssertionResultEntry(key, value);
        }

        public static void updateResultEntry(int key, string value)
        {
            Console.WriteLine("WeavingAssertionHandler.updateResultEntry");
            //deepTestConnectionSkeleton(key, value);
        }

        public static void stopwatchResultHook(ref Stopwatch s)
        {
            Console.WriteLine("StopWatchResultEntry === " + s.ElapsedMilliseconds);
            //deepTestConnectionSkeleton(0, (s.ElapsedMilliseconds / 1000.0).ToString());
        }
    }
}

