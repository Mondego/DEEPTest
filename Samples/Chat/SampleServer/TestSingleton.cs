using System;

namespace SampleServer
{
	public sealed class TestSingleton
	{
		private static readonly TestSingleton instance = new TestSingleton();

		static TestSingleton() {}
		private TestSingleton()
		{
		}

		public static TestSingleton Instance { get { return instance; } }

		public void Message(string arg)
		{
			Console.WriteLine("Messenger for test singleton, hard-coded " + arg);
		}
	}
}

