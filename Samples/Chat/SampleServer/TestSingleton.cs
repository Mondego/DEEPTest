using System;

namespace ChatServer
{
	/**
	 * This is a local copy of a Singleton like the one used by the FlowTest runtime. 
	 * 
	 * Usage is:
	 * TestSingleton.Instance.YourMethodHere(args);
	 */

	public sealed class TestSingleton
	{
		private static readonly TestSingleton instance = new TestSingleton();

		static TestSingleton() {}
		private TestSingleton()
		{
			// Constructor stuff here
		}

		public static TestSingleton Instance
		{
			get {
				return instance;
			}
		}

		// YourMethodHere
	}
}

