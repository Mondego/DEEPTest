using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FlowTest
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
					Console.WriteLine("[PID {0} {1}] {2}", 
						process.Id, new FileInfo(targetPath).Name,
						e.Data);
				}
			};
		}

		public void Start()
		{
			process.Start();
			process.BeginOutputReadLine();
			ProcessStreamInterface = process.StandardInput;
			Thread.Sleep (1000);
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

