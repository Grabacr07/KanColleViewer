using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class PresetShipData : NotificationObject
	{
		private Ship ship => KanColleClient.Current.Homeport.Organization.Ships.Where(x => x.Value.Id == this.ShipId).SingleOrDefault().Value ?? null;
		public string Name => ship?.Info?.Name ?? "？？？";
		public string TypeName => ship?.Info?.ShipType?.Name ?? "？？？";
		public int Level => ship?.Level ?? 0;

		public int[] SlotsId { get; set; }
		public int ExSlotId { get; set; }

		#region ShipId 프로퍼티
		private int _ShipId { get; set; }
		public int ShipId
		{
			get { return this._ShipId; }
			set
			{
				if (this._ShipId != value)
				{
					this._ShipId = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Slots 프로퍼티
		private IEnumerable<SlotItem> _Slots { get; set; }
		public IEnumerable<SlotItem> Slots
		{
			get { return this._Slots; }
			set
			{
				if (this._Slots != value)
				{
					this._Slots = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ExSlot 프로퍼티
		private SlotItem _ExSlot { get; set; }
		public SlotItem ExSlot
		{
			get { return this._ExSlot; }
			set
			{
				if (this._ExSlot != value)
				{
					this._ExSlot = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.ExSlotExist));
				}
			}
		}

		public bool ExSlotExist => this.ExSlot != null && this.ExSlot.Id > 0;
		#endregion

		public PresetShipData()
		{
			this.ShipId = -1;
			this.Slots = new SlotItem[0];
			this.ExSlot = null;
		}
		public PresetShipData(Ship ship, int[] slots, int exslot)
		{
			this.ShipId = ship.Id;
			this.SlotsId = slots;
			this.ExSlotId = exslot;

			this.UpdateShipData();
		}

		public string Serialize()
		{
			return string.Format(
				"{0}\t{1}\t{2}",
				this.ShipId,
				string.Join(",", this.SlotsId),
				this.ExSlotId
			);
		}
		public void Deserialize(string Data)
		{
			if (Data == null || (Data.Trim().Length == 0))
				return;

			string[] part = Data.Split('\t');
			int shipId = -1;
			int[] shipSlots;
			int shipExSlot = -1;

			int.TryParse(part[0], out shipId);
			shipSlots = part[1].Split(',')
				.Select(x =>
				{
					int y;
					if (int.TryParse(x, out y)) return (int?)y;
					else return null;
				})
				.Where(x => x.HasValue)
				.Select(x => x.Value)
				.ToArray();
			int.TryParse(part[2], out shipExSlot);

			this.ShipId = shipId;
			this.SlotsId = shipSlots;
			this.ExSlotId = shipExSlot;

			this.UpdateShipData();
		}

		private void UpdateShipData()
		{
			var homeport = KanColleClient.Current.Homeport;

			var slots = new List<SlotItem>();
			foreach(var id in this.SlotsId)
			{
				var item = homeport.Itemyard.SlotItems.SingleOrDefault(x => x.Value.Id == id).Value;
				if (item == null) continue;

				slots.Add(item);
			}
			this.Slots = slots.ToArray();
			this.ExSlot = homeport.Itemyard.SlotItems
				.SingleOrDefault(x => x.Value.Id == ExSlotId)
				.Value ?? null;
		}
	}

	public class PresetFleetData : ViewModel
	{
		#region FleetName 프로퍼티
		private string _FleetName { get; set; }
		public string FleetName
		{
			get { return this._FleetName; }
			set
			{
				if (this._FleetName != value)
				{
					this._FleetName = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Ships 프로퍼티
		private IReadOnlyCollection<PresetShipData> _Ships;
		public IReadOnlyCollection<PresetShipData> Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public PresetFleetData()
		{
		}
		public PresetFleetData(string FleetName)
		{
			this.FleetName = FleetName;
		}

		public string Serialize()
		{
			var result = this.FleetName + "\t";
			result += string.Join("\t", this.Ships.Select(x => x.Serialize()));
			return result;
		}
		public void Deserialize(string Data)
		{
			var datas = Data.Split('\t');
			int count = (datas.Length - 1) / 3;
			List<PresetShipData> list = new List<PresetShipData>();

			this.FleetName = datas[0];
			for (var i = 0; i < count; i++)
			{
				var item = new PresetShipData();
				item.Deserialize(string.Format(
					"{0}\t{1}\t{2}",
					datas[1 + i * 3],
					datas[1 + i * 3 + 1],
					datas[1 + i * 3 + 2]
				));
				list.Add(item);
			}

			this.Ships = list.ToArray();
		}
	}
}
