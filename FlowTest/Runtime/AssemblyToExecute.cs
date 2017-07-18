using System;
using System.Threading;

namespace FlowTest
{
	public class AssemblyToExecute
	{
		public int nSecondsRequiredForAssemblyStartup { get; }
		public string executionPath { get; }

		private string[] executionArguments;
		private ProcessWithIOHandler mAssemblyComponent;

		public AssemblyToExecute (string assemblyExecutionPath, int nSecondsForStartup, string[] arguments)
		{
			nSecondsRequiredForAssemblyStartup = nSecondsForStartup;
			executionPath = assemblyExecutionPath;
			executionArguments = arguments;
			mAssemblyComponent = new ProcessWithIOHandler (executionPath, executionArguments);
		}

		public void Start()
		{
			mAssemblyComponent.Start();
			Thread.Sleep(nSecondsRequiredForAssemblyStartup * 1000);
		}

		public void Stop()
		{
			mAssemblyComponent.Stop();
		}
	}
}

