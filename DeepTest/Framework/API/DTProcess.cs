using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace DeepTestFramework
{
	public class DTProcess
	{
        public Process p { get; }
		private StreamWriter ProcessStreamInterface;
        private string exePath;

        public DTProcess (string targetPath, string arguments, string workingdir = null)
		{
            exePath = targetPath;

			p = new Process();
            p.StartInfo.FileName = exePath;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;

			if (workingdir != null) {
				p.StartInfo.WorkingDirectory = workingdir;
			}

			p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
                if (e.Data.Length > 0)
                {
                    Console.WriteLine(e.Data.ToString().Trim());
                }
			};

            p.ErrorDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e) {
                if (e.Data.Length > 0) {
                    Console.WriteLine("[PID {0} {1}] {2}", 
                        p.Id, new FileInfo(targetPath).Name,
                        e.Data.ToString().Trim());
                }
            };
           
		}

        public void Start(int wait = 0)
		{
            try 
            {
                if (wait > 0) {
                    Thread.Sleep(wait * 1000);
                }

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                ProcessStreamInterface = p.StandardInput;
                Thread.Sleep(1000);
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
		}

		public void SendMessageToComponentConsole(string msg)
		{
			ProcessStreamInterface.WriteLine(msg);
		}

		public void Stop()
		{
			ProcessStreamInterface.Close();
            if (!p.HasExited) {
                p.CloseMainWindow();
                p.WaitForExit();
                p.Dispose();
            }
		}
	}
}

