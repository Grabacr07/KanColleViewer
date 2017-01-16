using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MetroTrilithon.Mvvm;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class PresetFleetAddWindowViewModel : WindowViewModel
	{
		private PresetFleetWindowViewModel _ViewModel;

		#region FleetName 변경 통지 프로퍼티
		private string _FleetName;

		public string FleetName
		{
			get
			{ return this._FleetName; }
			set
			{
				if (this._FleetName == value)
					return;
				this._FleetName = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		public PresetFleetAddWindowViewModel(PresetFleetWindowViewModel viewmodel)
		{
			this._ViewModel = viewmodel;
			this._FleetName = "";
		}

		public void AddByFleet(string data)
		{
			int deckid;
			if (!int.TryParse(data, out deckid)) return;

			if (deckid > 0)
			{
				var KanColleFleets = KanColleClient.Current.Homeport.Organization.Fleets;
				if (KanColleClient.Current.IsStarted == false || KanColleFleets.Count < deckid || KanColleFleets[deckid].Ships.Count() < 1) return;

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.Where(x => x.Value.Id == deckid).Single().Value;
				var ships = fleet.Ships;

				var item = new PresetFleetData();

				List<string> shipDataList = new List<string>();
				foreach(var ship in ships)
				{
					shipDataList.Add(string.Format(
						"{0}\t{1}\t{2}",
						ship.Id,
						string.Join(",", ship.Slots.Where(x => x.Equipped).Select(x => x.Item.Id)),
						(ship.ExSlotExists && ship.ExSlot.Equipped) ? ship.ExSlot.Item.Id : -1
					));
				}

				item.Deserialize(
					string.Format(
						"{0}\t{1}",
						string.IsNullOrEmpty(this.FleetName) ? fleet.Name : this.FleetName,
						string.Join("\t", shipDataList)
					)
				);
				this._ViewModel.AddFleet(item);
				Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			}
		}

		public void Cancel()
		{
			Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
		}
	}
}
