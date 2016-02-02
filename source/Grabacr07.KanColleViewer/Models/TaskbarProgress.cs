using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Shell;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class TaskbarProgress : NotificationObject, ITaskbarProgress
	{
		public static ITaskbarProgress[] Features => PluginService.Current.Get<ITaskbarProgress>();

		private ITaskbarProgress current;

		string ITaskbarProgress.Id
		{
			get { throw new NotSupportedException(); }
		}

		string ITaskbarProgress.DisplayName
		{
			get { throw new NotSupportedException(); }
		}

		#region State 変更通知プロパティ

		private TaskbarItemProgressState _State;

		public TaskbarItemProgressState State
		{
			get { return this._State; }
			set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Value 変更通知プロパティ

		private double _Value;

		public double Value
		{
			get { return this._Value; }
			set
			{
				if (!this._Value.Equals(value))
				{
					this._Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public event EventHandler Updated;

		public TaskbarProgress()
		{
			GeneralSettings.TaskbarProgressSource.Subscribe(x => this.Change(x));
		}

		public void Change(string id)
		{
			this.Change(Features.FirstOrDefault(x => x.Id == id));
		}

		public void Change(ITaskbarProgress progress)
		{
			if (this.current != null)
			{
				this.current.Updated -= this.CurrentOnUpdated;
			}

			if (progress == null)
			{
				this.current = null;
				this.State = TaskbarItemProgressState.None;
				this.Value = .0;
			}
			else
			{
				this.current = progress;
				this.current.Updated += this.CurrentOnUpdated;

				this.State = progress.State;
				this.Value = progress.Value;
			}

			this.Updated?.Invoke(this, EventArgs.Empty);
			this.RaisePropertyChanged(nameof(TaskbarProgress));
		}

		private void CurrentOnUpdated(object sender, EventArgs eventArgs)
		{
			var progress = (ITaskbarProgress)sender;

			this.State = progress.State;
			this.Value = progress.Value;

			this.Updated?.Invoke(this, EventArgs.Empty);
			this.RaisePropertyChanged(nameof(TaskbarProgress));
		}
	}
}
