using System;

namespace SampleServer
{
	public static class TestThreadsafe
	{
		private static Object theLock = new Object();

		public static void DoMessage(string msg)
		{
			Console.WriteLine(msg);
			lock (theLock)
			{
				onLock();
			}
		}

		public static void onLock()
		{
		}
	}
}