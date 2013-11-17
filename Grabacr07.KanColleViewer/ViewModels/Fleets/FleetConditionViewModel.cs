using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Fleets
{
	public class FleetConditionViewModel : ViewModel
	{
		private readonly FleetCondition source;

		public bool IsInRecovering
		{
			get { return this.source.IsInRecovering; }
		}

		public string RevivalTime
		{
			get { return this.source.RevivalTime.HasValue ? this.source.RevivalTime.Value.LocalDateTime.ToString("HH:mm") : "--:--"; }
		}

		public string Remaining
		{
			get { return this.source.Remaining.HasValue ? this.source.Remaining.Value.ToString(@"mm\:ss") : "--:--:--"; }
		}

		#region IsNotifyCompleted 変更通知プロパティ

		private bool _IsNotifyCompleted;

		public bool IsNotifyCompleted
		{
			get { return this._IsNotifyCompleted; }
			set
			{
				if (this._IsNotifyCompleted != value)
				{
					this._IsNotifyCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public FleetConditionViewModel(FleetCondition condition)
		{
			this.source = condition;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(condition, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));

			if (Helper.IsWindows8OrGreater)
			{
				condition.Recovered += (sender, args) =>
				{
					if (this.IsNotifyCompleted)
					{
						Toast.Show(
							"疲労回復完了",
							"「" + "" + "」の全艦娘の疲労が回復しました。",
							() => this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate")));
					}
				};
			}
		}
	}
}
