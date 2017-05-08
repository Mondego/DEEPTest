using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FlowTestAPI
{
	public class TargetComponentRuntime
	{
		private Process process;
		private StreamWriter ProcessStreamInterface;

		public TargetComponentRuntime (string targetPath, string[] targetArguments)
		{
			process = new Process();
			process.StartInfo.FileName = targetPath;
			process.StartInfo.Arguments = string.Join(" ", targetArguments);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				if (e.Data.Length > 0)
				{
					Console.WriteLine("[Debug] " + e.Data);
				}
			};
		}

		public void Start()
		{
			process.Start();
			process.BeginOutputReadLine();
			ProcessStreamInterface = process.StandardInput;
			Thread.Sleep (3000);
		}

		public void SendMessageToComponentConsole(string msg)
		{
			ProcessStreamInterface.WriteLine(msg);
			Thread.Sleep(1000);
		}

		public void Stop()
		{
			ProcessStreamInterface.Close();
			process.CloseMainWindow();
			process.WaitForExit();
			process.Dispose();
			Thread.Sleep(1000);
		}

		public int GetProcessId()
		{
			return process.Id;
		}
	}
}

