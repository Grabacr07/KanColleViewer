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

		public enum GCType
		{
			GCAll,
			GCOptimized
		}

		private bool GCWorking { get; set; } = false;
		private Thread GCThread { get; }

		private int GCCount => KanColleSettings.MemoryOptimizePeriod;

		private static bool _GCRequested { get; set; } = false;
		private static int _GCGen { get; set; } = -1;
		private static GCType _GCType { get; set; } = GCType.GCOptimized;

		public static void GCRequest(int gen = -1, GCType type = GCType.GCOptimized)
		{
			GCWorker._GCRequested = true;
			GCWorker._GCGen = gen;
			GCWorker._GCType = type;
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
					if (cnt == 0 || GCWorker._GCRequested)
					{
						cnt = GCCount;
						this.Collect(GCWorker._GCGen, GCWorker._GCType);
					}
				}
			});
			GCThread.Priority = ThreadPriority.BelowNormal;

			GCWorking = true;
			GCThread.Priority = ThreadPriority.BelowNormal;
			GCThread.Start();
		}

		private void Collect(int gen = -1, GCType type = GCType.GCOptimized)
		{
			if (gen == -1) gen = GC.MaxGeneration;

			// GC 가 Request 되었다면 예외적으로 수행
			if (!KanColleSettings.UseMemoryOptimize && !GCWorker._GCRequested) return;
			GCWorker._GCRequested = false;

			GC.Collect(gen, type == GCType.GCOptimized ? GCCollectionMode.Optimized : GCCollectionMode.Forced);
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
