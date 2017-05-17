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
		private static int GCGen { get; set; } = -1;
		public static void GCRequest(int gen = -1)
		{
			GCWorker.GCRequested = true;
			GCWorker.GCGen = gen;
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
						this.Collect(GCWorker.GCGen);
					}
				}
			});
			GCThread.Priority = ThreadPriority.BelowNormal;

			GCWorking = true;
			GCThread.Priority = ThreadPriority.BelowNormal;
			GCThread.Start();
		}

		private void Collect(int gen = -1)
		{
			if (gen == -1) gen = GC.MaxGeneration;

			// GC 가 Request 되었다면 예외적으로 수행
			if (!KanColleSettings.UseMemoryOptimize && !GCWorker.GCRequested) return;
			GCWorker.GCRequested = false;

			GC.Collect(gen, GCCollectionMode.Optimized);
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
