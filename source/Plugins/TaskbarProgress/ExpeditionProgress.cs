using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Shell;
using System.Windows.Threading;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.Properties;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Linq;
using MetroTrilithon.Mvvm;
using StatefulModel;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITaskbarProgress))]
	[Export(typeof(ISettings))]
	[ExportMetadata("Guid", guid)]
	[ExportMetadata("Title", "タスク バー遠征モニター")]
	[ExportMetadata("Description", "遠征の状況をタスク バー インジケーターに報告します。")]
	[ExportMetadata("Version", "1.1")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class ExpeditionProgress : IPlugin, ITaskbarProgress, ISettings, IDisposableHolder
	{
		private const string guid = "C8BF00A6-9FD4-4CC4-8FC5-ECCC5675CDEB";

		private readonly MultipleDisposable compositDisposable = new MultipleDisposable();
		private ExpeditionWrapper[] wrappers;
		private bool initialized;

		public string Id => guid + "-1";

		public string DisplayName => "遠征状況";

		public TaskbarItemProgressState State { get; private set; }

		public double Value { get; private set; }

		public bool ErrorIfAllWaiting
		{
			get { return Settings.Default.ErrorIfAllWaiting; }
			set
			{
				Settings.Default.ErrorIfAllWaiting = value;
				Settings.Default.Save();
				this.Update();
			}
		}

		object ISettings.View => new ExpeditionProgressSettings { DataContext = this, };

		public event EventHandler Updated;

		public void Initialize()
		{
			KanColleClient.Current
				.Subscribe(nameof(KanColleClient.IsStarted), () => this.InitializeCore(), false)
				.AddTo(this);

			var timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(Settings.Default.Interval), };
			timer.Tick += (sender, e) => this.Update();
			timer.Start();

			Disposable.Create(() => Settings.Default.Save()).AddTo(this);
		}

		private void InitializeCore()
		{
			if (this.initialized) return;

			var homeport = KanColleClient.Current.Homeport;
			if (homeport != null)
			{
				this.initialized = true;

				homeport.Organization
					.Subscribe(nameof(Organization.Fleets), this.UpdateExpeditions)
					.AddTo(this);
			}
		}

		public void UpdateExpeditions()
		{
			if (!this.initialized) return;

			if (this.wrappers != null)
			{
				foreach (var wrapper in this.wrappers) wrapper.Dispose();
			}

			this.wrappers = KanColleClient.Current.Homeport.Organization.Fleets
				.Skip(1)
				.Select(x => new { x.Value.Id, x.Value.Expedition, })
				.Where(a => a.Expedition != null)
				.Select(a => new ExpeditionWrapper(a.Id, a.Expedition).AddTo(this))
				.Do(x => x.Subscribe(nameof(ExpeditionWrapper.State), () => this.Update()).AddTo(this))
				.ToArray();

			this.Update();
		}

		public void Update()
		{
			if (!this.initialized) return;

			if (this.wrappers.Length == 0)
			{
				this.State = TaskbarItemProgressState.None;
				this.Value = .0;
			}
			else if (this.wrappers.Any(x => x.State == ExpeditionState.Returned))
			{
				this.State = TaskbarItemProgressState.Indeterminate;
				this.Value = 1.0;
			}
			else
			{
				var target = this.wrappers.Aggregate(Early);
				if (target.Source.Remaining.HasValue && target.Source.ReturnTime.HasValue)
				{
					var state = this.wrappers.Any(x => x.State == ExpeditionState.Waiting)
						? TaskbarItemProgressState.Paused
						: TaskbarItemProgressState.Normal;
					var start = target.Source.ReturnTime.Value.Subtract(TimeSpan.FromMinutes(target.Source.Mission.RawData.api_time)); // 開始時間
					var value = DateTimeOffset.Now.Subtract(start).TotalMinutes / target.Source.Mission.RawData.api_time;

					this.State = state;
					this.Value = value;
				}
				else
				{
					this.State = TaskbarItemProgressState.Error;
					this.Value = this.ErrorIfAllWaiting ? 1.0 : .0;
				}
			}

			this.Updated?.Invoke(this, EventArgs.Empty);
		}

		private static ExpeditionWrapper Early(ExpeditionWrapper wrapper1, ExpeditionWrapper wrapper2)
		{
			// 2 つの遠征を比較して早く帰ってくるほうを返すやつ

			return wrapper1.Source.IsInExecution
				? wrapper2.Source.IsInExecution
					? wrapper1.Source.ReturnTime < wrapper2.Source.ReturnTime
						? wrapper1
						: wrapper2
					: wrapper1
				: wrapper2;
		}

		public void Dispose() => this.compositDisposable.Dispose();
		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositDisposable;
	}
}
