using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Grabacr07.KanColleViewer.Models
{
	internal class GCWorker : IDisposable
	{
		protected bool GCWorking { get; set; } = false;
		protected Thread GCThread { get; }

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
						GC.Collect();
					}
				}
			});

			GCWorking = true;
			GCThread.Start();
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
