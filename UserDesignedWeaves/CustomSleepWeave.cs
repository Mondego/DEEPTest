using System;
using System.Threading;

namespace UserDesignedWeaves
{
	public class CustomSleepWeave
	{
		public CustomSleepWeave ()
		{
			Console.WriteLine ("[DEBUG] User designed code to weave in.");
		}

		public void Weave()
		{
			Thread.Sleep (1000);
		}
	}
}

