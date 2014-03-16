using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Docks
{
	public class RepairyardViewModel : TabItemViewModel
	{
		#region Docks 変更通知プロパティ

		private RepairingDockViewModel[] _Docks;

		public RepairingDockViewModel[] Docks
		{
			get { return this._Docks; }
			set
			{
				if (!Equals(this._Docks, value))
				{
					this._Docks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsNotifyCompleted 変更通知プロパティ

		public bool IsNotifyCompleted
		{
			get { return Settings.Current.NotifyRepairingCompleted; }
			set
			{
				if (Settings.Current.NotifyRepairingCompleted != value)
				{
					Settings.Current.NotifyRepairingCompleted = value;
					this.Docks.ForEach(x => x.IsNotifyCompleted = value);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public bool IsSupportedNotification
		{
			get { return Helper.IsWindows8OrGreater; }
		}


		public RepairyardViewModel()
		{
			this.Name = Properties.Resources.Repairyard;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Repairyard)
			{
				{ "Docks", (sender, args) => this.Update() },
			});
			this.Update();
		}

		private void Update()
		{
			this.Docks = KanColleClient.Current.Homeport.Repairyard.Docks.Select(kvp => new RepairingDockViewModel(kvp.Value)).ToArray();
			this.Docks.ForEach(x => x.IsNotifyCompleted = this.IsNotifyCompleted);
		}
	}
}
