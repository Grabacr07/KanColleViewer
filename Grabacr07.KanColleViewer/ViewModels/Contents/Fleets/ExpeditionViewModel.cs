using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class ExpeditionViewModel : ViewModel
	{
		private readonly Expedition source;

		public bool IsInExecution
		{
			get { return this.source.IsInExecution; }
		}

		public string ReturnTime
		{
			get
			{
				return this.source.ReturnTime.HasValue
					? this.source.ReturnTime.Value.LocalDateTime.ToString("MM/dd HH:mm")
					: "--/-- --:--";
			}
		}

		public string Remaining
		{
			get
			{
				return this.source.Remaining.HasValue
					? string.Format("{0:D2}:{1}",
						(int)this.source.Remaining.Value.TotalHours,
						this.source.Remaining.Value.ToString(@"mm\:ss"))
					: "--:--:--";
			}
		}

		#region IsNotifyReturned 変更通知プロパティ

		private bool _IsNotifyReturned;

		public bool IsNotifyReturned
		{
			get { return this._IsNotifyReturned; }
			set
			{
				if (this._IsNotifyReturned != value)
				{
					this._IsNotifyReturned = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ExpeditionViewModel(Expedition expedition)
		{
			this.source = expedition;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(expedition, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));

			if (Toast.IsSupported)
			{
				expedition.Returned += (sender, args) =>
				{
					if (this.IsNotifyReturned)
					{
						Toast.Show(
							Resources.Expedition_NotificationMessage_Title,
							string.Format(Resources.Expedition_NotificationMessage, args.FleetName),
							() => App.ViewModelRoot.Activate());
					}
				};
			}
			else
			{
				expedition.Returned += (sender, args) =>
				{
					if (this.IsNotifyReturned)
					{
						NotifyIconWrapper.Show(
							Resources.Expedition_NotificationMessage_Title,
							string.Format(Resources.Expedition_NotificationMessage, args.FleetName));
					}
				};
			}
		}
	}
}
