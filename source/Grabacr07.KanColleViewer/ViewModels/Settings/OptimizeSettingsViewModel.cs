using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.Messaging.IO;
using MetroTrilithon.Mvvm;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class OptimizeSettingsViewModel : ViewModel
	{
		#region CurrentMemory 변경통지 프로퍼티

		private string _CurrentMemory { get; set; }
		public string CurrentMemory
		{
			get { return this._CurrentMemory; }
			set
			{
				if (_CurrentMemory != value)
				{
					this._CurrentMemory = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private class MemoryChecker : TimerNotifier
		{
			#region MemoryCurrent 변경통지 프로퍼티

			private long _MemoryCurrent;
			public long MemoryCurrent
			{
				get { return this._MemoryCurrent; }
				set
				{
					if (_MemoryCurrent != value)
					{
						this._MemoryCurrent = value;
						this.RaisePropertyChanged();
					}
				}
			}

			#endregion

			protected override void Tick()
			{
				base.Tick();
				MemoryCurrent = Process.GetCurrentProcess().WorkingSet64;
			}
		}

		MemoryChecker checker;

		public OptimizeSettingsViewModel()
		{
			this.checker = new MemoryChecker();
			checker.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(checker.MemoryCurrent))
					CurrentMemory = MeasureSize(checker.MemoryCurrent);
			};
			CurrentMemory = "-";
		}

		public void RequestGC()
		{
			GCWorker.GCRequest();
		}

		private string MeasureSize(long bytes)
		{
			double size = bytes;
			string[] units = new string[] { "bytes", "KBs", "MBs", "GBs", "TBs" };
			int unit = 0;

			while (size >= 1000)
			{
				size /= 1000.0;
				unit++;
			}
			return Math.Round(size, 2) + " " + units[unit];
		}
	}
}
