using System;

namespace FlowTestTemplates
{
	public static class PublicStaticClassWithLocking
	{
		private static Object locking = new Object();

		public static void DoMessage(string message)
		{
			lock (locking)
			{
				onLock(message);
			}
		}

		public static void onLock(string value)
		{
		}
	}
}

