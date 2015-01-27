using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using Livet.Messaging;
using System;
using System.Linq;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipyardViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Properties.Resources.Shipyard; }
			protected set { throw new NotImplementedException(); }
		}

		#region RepairingDocks 変更通知プロパティ

		private RepairingDockViewModel[] _RepairingDocks;

		public RepairingDockViewModel[] RepairingDocks
		{
			get { return this._RepairingDocks; }
			set
			{
				if (!Equals(this._RepairingDocks, value))
				{
					this._RepairingDocks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BuildingDocks 変更通知プロパティ

		private BuildingDockViewModel[] _BuildingDocks;

		public BuildingDockViewModel[] BuildingDocks
		{
			get { return this._BuildingDocks; }
			set
			{
				if (!Equals(this._BuildingDocks, value))
				{
					this._BuildingDocks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public CreatedSlotItemViewModel CreatedSlotItem { get; private set; }


		public ShipyardViewModel()
		{
			this.CreatedSlotItem = new CreatedSlotItemViewModel();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Repairyard)
			{
				{ "Docks", (sender, args) => this.UpdateRepairingDocks() },
			});
			this.UpdateRepairingDocks();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Dockyard)
			{
				{ "Docks", (sender, args) => this.UpdateBuildingDocks() },
				{ "CreatedSlotItem", (sender, args) => this.UpdateSlotItem() },
			});
			this.UpdateBuildingDocks();
		}


		private void UpdateRepairingDocks()
		{
			this.RepairingDocks = KanColleClient.Current.Homeport.Repairyard.Docks.Select(kvp => new RepairingDockViewModel(kvp.Value)).ToArray();
		}

		private void UpdateBuildingDocks()
		{
			this.BuildingDocks = KanColleClient.Current.Homeport.Dockyard.Docks.Select(kvp => new BuildingDockViewModel(kvp.Value)).ToArray();
		}

		private void UpdateSlotItem()
		{
			this.CreatedSlotItem.Update(KanColleClient.Current.Homeport.Dockyard.CreatedSlotItem);
		}

		public void ShowNdockShipCatalog()
		{
			var catalog = new NeedNdockShipCatalogWindowViewModel();
			var message = new TransitionMessage(catalog, "Show/NeedNdockShipCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}
	}
}
