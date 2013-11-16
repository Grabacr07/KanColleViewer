﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels
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
			get { return source.ReturnTime.HasValue ? source.ReturnTime.Value.LocalDateTime.ToString("MM/dd HH:mm") : "--/-- --:--:--"; }
		}

		public string Remaining
		{
			get
			{
				if (source.Remaining.HasValue)
				{
					var remaining = source.Remaining.Value;
					return string.Format(remaining.ToString(@"'{0:00}'\:mm\:ss"), (int)remaining.TotalHours);
				}
				else
				{
					return "--:--:--";
				}
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

			if (Helper.IsWindows8OrGreater)
			{
				expedition.Returned += (sender, args) =>
				{
					if (this.IsNotifyReturned)
					{
						Toast.Show(
							"遠征完了",
							"'" + args.FleetName + "' が遠征から帰投しました。",
							() => this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate")));
					}
				};
			}
		}
	}
}
