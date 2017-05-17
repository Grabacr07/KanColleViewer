using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using Livet.EventListeners;
using MetroTrilithon.Mvvm;

using Grabacr07.KanColleViewer.Models;

using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class DictionaryViewModel : WindowViewModel
	{
		internal static IEnumerable<kcsapi_mst_shipgraph> shipGraphics
			=> KanColleClient.Current.Master.RawData?.api_mst_shipgraph ?? new kcsapi_mst_shipgraph[0];

		internal static IEnumerable<ships_nedb> shipInfos { get; private set; }
			= new ships_nedb[0];

		static DictionaryViewModel()
		{
			new Thread(async () =>
			{
				using (var client = new HttpClient())
				{
					try
					{
						var response = await client.GetAsync("https://raw.githubusercontent.com/Diablohu/WhoCallsTheFleet/master/app-db/ships.nedb");
						if (!response.IsSuccessStatusCode) return;

						var json = await response.Content.ReadAsStringAsync();
						var lines = json.Split(new char[] { '\r', '\n' }).Where(x => x.Length > 0);

						List<ships_nedb> infoList = new List<ships_nedb>();

						foreach (var line in lines)
						{
							var bytes = Encoding.UTF8.GetBytes(line);

							DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
							settings.UseSimpleDictionaryFormat = true;

							var serializer = new DataContractJsonSerializer(typeof(ships_nedb), settings);
							using (var stream = new MemoryStream(bytes))
							{
								var rawResult = serializer.ReadObject(stream) as ships_nedb;
								infoList.Add(rawResult);
							}
						}

						shipInfos = infoList.ToArray();
					}
					catch (HttpRequestException) { return; }
				}
			}).Start();
		}


		public IList<TabItemViewModel> TabItems { get; set; } = new TabItemViewModel[0]; // Empty List

		private TabItemViewModel _SelectedItem = null;
		public TabItemViewModel SelectedItem
		{
			get { return this._SelectedItem; }
			set
			{
				if (this._SelectedItem != value)
				{
					this._SelectedItem = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public DictionaryViewModel()
		{
			this.Title = "데이터 사전";

			this.TabItems = new List<TabItemViewModel>
			{
				new DictionaryShipViewModel().AddTo(this),
			};
			this.SelectedItem = this.TabItems.FirstOrDefault();
		}
	}

	public class DictionaryShipViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return "칸무스"; }
			protected set { throw new NotImplementedException(); }
		}

		#region Tab (List)
		public IList<ShipItemViewModel> ShipList { get; set; }

		private ShipItemViewModel _SelectedShip;
		public ShipItemViewModel SelectedShip
		{
			get { return this._SelectedShip; }
			set
			{
				if (this._SelectedShip != value)
				{
					this._SelectedShip = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public DictionaryShipViewModel()
		{
			this.ShipList = KanColleClient.Current.Master.Ships
				.OrderBy(x => x.Value.Id)
				.Select(x => new ShipItemViewModel(x.Value))
				.ToArray();
		}
	}

	public class ShipItemViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return _Name; }
			protected set { throw new NotImplementedException(); }
		}
		public string Id => this._Id;

		private string _Name { get; set; }
		private string _Id { get; set; }

		public KanColleWrapper.Models.ShipInfo Ship { get; }

		public string BadgeImage =>
			string.Format(
				"http://wolfgangkurz.github.io/KanColleAssets/ships/{0}/1.png",
				DictionaryViewModel.shipGraphics.FirstOrDefault(x => x.api_id == (this.Ship?.Id ?? 0))?.api_filename ?? "none"
			);

		#region Status
		public int HP => this.Ship?.RawData.api_taik?[0] ?? -1;

		public int MinArmor => this.Ship?.RawData.api_souk?[0] ?? -1;
		public int MaxArmor => this.Ship?.RawData.api_souk?[1] ?? -1;

		public int MinEvasion => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.evasion ?? -1;
		public int MaxEvasion => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.evasion_max ?? -1;

		public int TotalCarry => this.Ship?.RawData.api_maxeq?.Sum() ?? -1;

		///////////////////

		public int MinFire => this.Ship?.RawData.api_houg?[0] ?? -1;
		public int MaxFire => this.Ship?.RawData.api_houg?[1] ?? -1;

		public int MinTorpedo => this.Ship?.RawData.api_raig?[0] ?? -1;
		public int MaxTorpedo => this.Ship?.RawData.api_raig?[1] ?? -1;

		public int MinAA => this.Ship?.RawData.api_taik?[0] ?? -1;
		public int MaxAA => this.Ship?.RawData.api_taik?[1] ?? -1;

		public int MinASW => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.asw ?? -1;
		public int MaxASW => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.asw_max ?? -1;

		///////////////////

		public ShipSpeed Speed => ShipSpeedConverter.FromInt32(this.Ship?.RawData.api_soku ?? 0);
		public string SpeedText
			=> this.Speed == ShipSpeed.Immovable ? "육상기지"
				: this.Speed == ShipSpeed.Slow ? "저속"
				: this.Speed == ShipSpeed.Fast ? "고속"
				: this.Speed == ShipSpeed.Faster ? "고속+"
				: this.Speed == ShipSpeed.Fastest ? "초고속"
				: "???";

		public int Range => this.Ship?.RawData.api_leng ?? -1;
		public string RangeText
			=> this.Range == 1 ? "단거리"
				: this.Range == 2 ? "중거리"
				: this.Range == 3 ? "장거리"
				: this.Range == 4 ? "최장거리"
				: "???";

		public int MinLOS => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.los ?? -1;
		public int MaxLOS => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.los_max ?? -1;

		public int MinLuck => this.Ship?.RawData.api_luck?[0] ?? -1;
		public int MaxLuck => this.Ship?.RawData.api_luck?[1] ?? -1;

		///////////////////

		public int MaxFuel => this.Ship?.RawData.api_fuel_max ?? -1;
		public int MaxAmmo => this.Ship?.RawData.api_bull_max ?? -1;
		#endregion

		// Equipment
		public IList<SlotInfo> Slots
		{
			get
			{
				var list = new List<SlotInfo>();

				var info = DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0));
				if (info == null)
				{
					while (list.Count < (this.Ship?.SlotCount ?? 0))
						list.Add(new SlotInfo(null, true, this.Ship?.Slots?[list.Count] ?? -1));

					while (list.Count < 4) list.Add(new SlotInfo(null, false, 0));

					return list;
				}

				list = info.equip
					.Where(x =>
					{
						int id;
						if (x == null || x.Length == 0) return false;
						return int.TryParse(x, out id);
					})
					.Select((x, z) => {
						int id;
						if (!int.TryParse(x, out id)) return new SlotInfo(null, false, 0);

						var master = KanColleClient.Current.Master;
						var item = master.SlotItems.FirstOrDefault(y => y.Value.Id == id).Value;
						if (item == null) return new SlotInfo(null, false, 0);

						return new SlotInfo(item, true, this.Ship?.Slots[z] ?? -1);
					})
					.ToList();

				while (list.Count < (this.Ship?.SlotCount ?? 0))
					list.Add(new SlotInfo(null, true, this.Ship?.Slots[list.Count] ?? -1));

				while (list.Count < 4) list.Add(new SlotInfo(null, false, 0));

				return list;
			}
		}

		#region Modernize & Destroy
		public int ModernizeFire => this.Ship?.RawData.api_powup?[0] ?? 0;
		public int ModernizeTorpedo => this.Ship?.RawData.api_powup?[1] ?? 0;
		public int ModernizeAA => this.Ship?.RawData.api_powup?[2] ?? 0;
		public int ModernizeArmor => this.Ship?.RawData.api_powup?[3] ?? 0;

		public int DestroyFuel => this.Ship?.RawData.api_broken?[0] ?? 0;
		public int DestroyAmmo => this.Ship?.RawData.api_broken?[1] ?? 0;
		public int DestroySteel => this.Ship?.RawData.api_broken?[2] ?? 0;
		public int DestroyBauxite => this.Ship?.RawData.api_broken?[3] ?? 0;
		#endregion

		public ShipItemViewModel(KanColleWrapper.Models.ShipInfo ShipData)
		{
			this.Ship = ShipData;
			this._Name = this.Ship?.Name ?? "???";
			this._Id = this.Ship?.Id.ToString() ?? "???";
		}
	}
	public class SlotInfo
	{
		public SlotItemInfo Info { get; set; }
		public bool Available { get; set; }
		public int Carry { get; set; }

		public SlotInfo(SlotItemInfo Info, bool Available, int Carry)
		{
			this.Info = Info;
			this.Available = Available;
			this.Carry = Carry;
		}
	}

	#region Raw Models
	// ReSharper disable InconsistentNaming
	public class ships_nedb
	{
		public int id { get; set; }
		public int no { get; set; }

		public ships_nedb_stat stat { get; set; }
		public int[] slot { get; set; }
		public string[] equip { get; set; }
	}
	public class ships_nedb_stat
	{
		public int evasion { get; set; }
		public int evasion_max { get; set; }

		public int asw { get; set; }
		public int asw_max { get; set; }

		public int los { get; set; }
		public int los_max { get; set; }
	}
	// ReSharper restore InconsistentNaming
	#endregion
}
