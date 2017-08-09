using System;

namespace SampleServer
{
	public sealed class TestSingleton
	{
		private static readonly TestSingleton instance = new TestSingleton();

		static TestSingleton () {}
		private TestSingleton () {}

		public static TestSingleton Instance { get { return instance; }}

		public void SendResult(string message)
		{
			Console.WriteLine ("This is the non-woven result " + message);
		}
	}
}

