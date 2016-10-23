using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.Models
{
	internal sealed class GCWorker : IDisposable
	{
		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

		private bool GCWorking { get; set; } = false;
		private Thread GCThread { get; }

		private int GCCount => KanColleSettings.MemoryOptimizePeriod;

		private static bool GCRequested { get; set; } = false;
		public static void GCRequest()
		{
			GCWorker.GCRequested = true;
		}

		public GCWorker()
		{
			GCThread = new Thread(() =>
			{
				int cnt = GCCount;
				while (GCWorking)
				{
					Thread.Sleep(1000);
					if (!GCWorking) break;

					cnt--;
					if (cnt == 0 || GCWorker.GCRequested)
					{
						cnt = GCCount;
						this.Collect();
					}
				}
			});

			GCWorking = true;
			GCThread.Start();
		}
		private void Collect()
		{
			// GC 가 Request 되었다면 예외적으로 수행
			if (!KanColleSettings.UseMemoryOptimize && !GCWorker.GCRequested) return;
			GCWorker.GCRequested = false;

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
