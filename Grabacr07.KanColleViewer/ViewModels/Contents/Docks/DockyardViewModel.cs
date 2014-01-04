using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Docks
{
	public class DockyardViewModel : TabItemViewModel
	{
		#region Docks 変更通知プロパティ

		private BuildingDockViewModel[] _Docks;

		public BuildingDockViewModel[] Docks
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

		#region CanDisplayShipName 変更通知プロパティ

		public bool CanDisplayShipName
		{
			get { return Settings.Current.CanDisplayBuildingShipName; }
			set
			{
				if (Settings.Current.CanDisplayBuildingShipName != value)
				{
					Settings.Current.CanDisplayBuildingShipName = value;
					this.Docks.ForEach(x => x.CanDisplayShipName = value);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsNotifyCompleted 変更通知プロパティ

		public bool IsNotifyCompleted
		{
			get { return Settings.Current.NotifyBuildingCompleted; }
			set
			{
				if (Settings.Current.NotifyBuildingCompleted != value)
				{
					Settings.Current.NotifyBuildingCompleted = value;
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


		public DockyardViewModel()
		{
			this.Name = Properties.Resources.Dockyard;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Dockyard)
			{
				{ "Docks", (sender, args) => this.Update() },
			});
			this.Update();
		}

		private void Update()
		{
			this.Docks = KanColleClient.Current.Homeport.Dockyard.Docks.Select(kvp => new BuildingDockViewModel(kvp.Value)).ToArray();
			this.Docks.ForEach(x =>
			{
				x.CanDisplayShipName = this.CanDisplayShipName;
				x.IsNotifyCompleted = this.IsNotifyCompleted;
			});
		}
	}
}
