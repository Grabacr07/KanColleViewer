using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Grabacr07.KanColleViewer.Models
{
	internal sealed class GCWorker : IDisposable
	{
		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

		private bool GCWorking { get; set; } = false;
		private Thread GCThread { get; }

		public GCWorker()
		{
			GCThread = new Thread(() =>
			{
				int cnt = 60 * 5;
				while (GCWorking)
				{
					Thread.Sleep(1000);
					if (!GCWorking) break;

					cnt--;
					if (cnt == 0)
					{
						cnt = 60 * 5;
						this.Collect();
					}
				}
			});

			GCWorking = true;
			GCThread.Start();
		}
		private void Collect()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
		}

		~GCWorker()
		{
			this.Dispose();
		}
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			GCWorking = false;
		}
	}
}
