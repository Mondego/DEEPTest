using System;
using System.Threading;

namespace UserDesignedWeaves
{
	public class StaticMethodsCustomSleepWeave
	{
		public static void StaticMethodToWeaveThreadSleep()
		{
			Console.WriteLine ("[DEBUG] User designed code to weave in from static method.");
			Thread.Sleep (1000);
		}
	}
}

