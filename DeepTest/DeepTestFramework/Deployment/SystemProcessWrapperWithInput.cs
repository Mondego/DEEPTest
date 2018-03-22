using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace DeepTestFramework
{
    public class SystemProcessWrapperWithInput : IDisposable
	{
        public Process p { get; }
		private StreamWriter ProcessStreamInterface;
        private string exePath;

        public SystemProcessWrapperWithInput (string targetPath, string arguments, string workingdir = null)
		{
            exePath = targetPath;

            if (!File.Exists(exePath)) {
                throw new FileNotFoundException("InstrumentedProcess path " + targetPath);
            }

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

            Start();
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
                Thread.Sleep(5000);
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
		}

		public void ConsoleInput(string msg)
		{
			ProcessStreamInterface.WriteLine(msg);
		}

		public void Stop()
		{
            Console.WriteLine("Stopping P{0} {1}", p.Id, exePath);
			ProcessStreamInterface.Close();
            if (!p.HasExited) {
                p.CloseMainWindow();
                p.WaitForExit();
                p.Dispose();
            }
		}

        public void StopAfterNSeconds(int n)
        {
            Console.WriteLine("Stopping P{0} {1} in {2} seconds", p.Id, exePath, n);
            Thread.Sleep(n * 1000);
            Stop();
        }

        public void Dispose()
        {
            Stop();
        }
	}
}

