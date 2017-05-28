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

		internal static IEnumerable<ships_nedb> shipInfos { get; private set; } = new ships_nedb[0];
		internal static IEnumerable<ships_abyssal_info> abyssalShipInfos { get; private set; } = new ships_abyssal_info[0];

		static DictionaryViewModel()
		{
			new Thread(async () =>
			{
				#region Abyssal Informations
				using (var client = new HttpClient())
				{
					try
					{
						var response = await client.GetAsync("https://wolfgangkurz.github.io/KanColleAssets/abyssal_table.json");
						if (!response.IsSuccessStatusCode) return;

						var json = await response.Content.ReadAsStringAsync();
						var bytes = Encoding.UTF8.GetBytes(json);

						DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
						settings.UseSimpleDictionaryFormat = true;

						var serializer = new DataContractJsonSerializer(typeof(ships_abyssal_info[]), settings);
						using (var stream = new MemoryStream(bytes))
						{
							var rawResult = serializer.ReadObject(stream) as ships_abyssal_info[];
							abyssalShipInfos = rawResult;
						}
					}
					catch (HttpRequestException) { return; }
				}
				#endregion

				#region Ship datas (WhoCallsTheFleet)
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
				#endregion
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
					foreach (var item in this.TabItems)
						item.IsSelected = false;

					this._SelectedItem = value;
					this._SelectedItem.IsSelected = true;

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
				new DictionaryEquipmentViewModel().AddTo(this),
			};
			this.SelectedItem = this.TabItems.FirstOrDefault();
		}
	}

	#region For Ship Dictionary Tab
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

		private string _SearchText { get; set; }
		public string SearchText
		{
			get { return this._SearchText; }
			set
			{
				if (this._SearchText != value)
				{
					this._SearchText = value;
					this.RaisePropertyChanged();

					this.UpdateList();
				}
			}
		}
		#endregion

		public DictionaryShipViewModel()
		{
			UpdateList();
		}

		private void UpdateList()
		{
			this.ShipList = KanColleClient.Current.Master.Ships
				.OrderBy(x => x.Value.Id)
				.Where(x => x.Value.Name.IndexOf(this.SearchText ?? "", StringComparison.CurrentCultureIgnoreCase) >= 0)
				.Select(x => new ShipItemViewModel(this, x.Value))
				.ToArray();

			this.RaisePropertyChanged(nameof(this.ShipList));
		}
	}

	public class ShipItemViewModel : TabItemViewModel
	{
		private DictionaryShipViewModel Parent { get; }

		public override string Name
		{
			get { return _Name; }
			protected set { throw new NotImplementedException(); }
		}
		public string Id => this._Id;
		public string ShipType => this._ShipType;

		#region Level
		private int _Level { get; set; } = 1;
		public int Level
		{
			get { return _Level; }
			set
			{
				if (this._Level != value)
				{
					this._Level = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.Marriaged));
					this.UpdateAllValues();
				}
			}
		}
		#endregion

		public bool IsAbyssal => (this.Ship?.Id ?? 0) > 1500;
		public bool Marriaged => this.Level > 99 && (this.Ship?.Id ?? 0) <= 1500;

		private string _Name { get; set; }
		private string _Id { get; set; }
		private string _ShipType { get; set; }

		public KanColleWrapper.Models.ShipInfo Ship { get; }

		public string BadgeImage =>
			string.Format(
				"http://wolfgangkurz.github.io/KanColleAssets/ships/{0}/1.png",
				DictionaryViewModel.shipGraphics.FirstOrDefault(x => x.api_id == (this.Ship?.Id ?? 0))?.api_filename ?? "none"
			);

		#region Status
		public int MarriageHP
		{
			get
			{
				var id = this.Ship?.Id ?? -1;
				if (id <= 0) return 0;

				switch (id)
				{
					case 171: // Bismarck
						return 6;
					case 172: // Bismarck改
						return 5;
					case 173: // Bismarck zwei
					case 178: // Bismarck drei
						return 3;

					case 131: // 大和
						return 5;
					case 143: // 武蔵
						return 4;

					case 441: // Italia
					case 442: // Roma
						return 6;

					case 541: // 長門改二
						return 7;

					case 352: // 速吸改
						return 2;
				}

				if (this.HP >= 91) // 91 ~
					return 9;
				else if (this.HP >= 70) // 70 ~ 90
					return 8;
				else if (this.HP >= 50) // 50 ~ 69
					return 7;
				else if (this.HP >= 40) // 40 ~ 49
					return 6;
				else if (this.HP >= 30) // 30 ~ 39
					return 5;
				else if (this.HP >= 8) // 8 ~ 29
					return 4;
				else if (this.HP >= 6) // 6 ~ 7 (まるゆ)
					return 3;

				return 0;
			}
		}
		public int MarriageHPResult
			=> this.Marriaged ? this.MarriageHP : 0;

		public int AbyssalHP { get; set; }
		public int HP => this.Ship?.RawData.api_taik?[0] ?? -1;
		public int CurHP => this.HP == -1 ? -1
			: this.HP + (this.Marriaged ? this.MarriageHP : 0);

		public int AbyssalArmor { get; set; }
		public int MinArmor => this.Ship?.RawData.api_souk?[0] ?? -1;
		public int MaxArmor => this.Ship?.RawData.api_souk?[1] ?? -1;
		public int CurArmor => this.Level == 1 ? this.MinArmor
			: this.Level == 99 ? (int)Math.Floor(0.4 * (this.MaxArmor - this.MinArmor)) + this.MinArmor
			: this.MinArmor == -1 || this.MaxArmor == -1 ? -1
			: (int)Math.Floor((double)(this.MaxArmor - this.MinArmor) * this.Level / 99 * 0.4 + this.MinArmor);

		public int AbyssalEvasion { get; set; }
		public int MinEvasion => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.evasion ?? -1;
		public int MaxEvasion => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.evasion_max ?? -1;
		public int CurEvasion => this.Level == 1 ? this.MinEvasion
			: this.Level == 99 ? this.MaxEvasion
			: this.MinEvasion == -1 || this.MaxEvasion == -1 ? -1
			: (int)Math.Floor((double)(this.MaxEvasion - this.MinEvasion) * this.Level / 99 + this.MinEvasion);

		public int AbyssalCarry { get; set; }
		public int TotalCarry => this.Ship?.RawData.api_maxeq?.Sum() ?? -1;

		///////////////////

		public int AbyssalFire { get; set; }
		public int MinFire => this.Ship?.RawData.api_houg?[0] ?? -1;
		public int MaxFire => this.Ship?.RawData.api_houg?[1] ?? -1;
		public int CurFire => this.Level == 1 ? this.MinFire
			: this.Level == 99 ? (int)Math.Floor(0.4 * (this.MaxFire - this.MinFire)) + this.MinFire
			: this.MinFire == -1 || this.MaxFire == -1 ? -1
			: (int)Math.Floor((double)(this.MaxFire - this.MinFire) * this.Level / 99 * 0.4 + this.MinFire);

		public int AbyssalTorpedo { get; set; }
		public int MinTorpedo => this.Ship?.RawData.api_raig?[0] ?? -1;
		public int MaxTorpedo => this.Ship?.RawData.api_raig?[1] ?? -1;
		public int CurTorpedo => this.Level == 1 ? this.MinTorpedo
			: this.Level == 99 ? (int)Math.Floor(0.4 * (this.MaxTorpedo - this.MinTorpedo)) + this.MinTorpedo
			: this.MinTorpedo == -1 || this.MaxTorpedo == -1 ? -1
			: (int)Math.Floor((double)(this.MaxTorpedo - this.MinTorpedo) * this.Level / 99 * 0.4 + this.MinTorpedo);

		public int AbyssalAA { get; set; }
		public int MinAA => this.Ship?.RawData.api_taik?[0] ?? -1;
		public int MaxAA => this.Ship?.RawData.api_taik?[1] ?? -1;
		public int CurAA => this.Level == 1 ? this.MinAA
			: this.Level == 99 ? (int)Math.Floor(0.4 * (this.MaxAA - this.MinAA)) + this.MinAA
			: this.MinAA == -1 || this.MaxAA == -1 ? -1
			: (int)Math.Floor((double)(this.MaxAA - this.MinAA) * this.Level / 99 * 0.4 + this.MinAA);

		public int AbyssalASW { get; set; }
		public int MinASW => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.asw ?? -1;
		public int MaxASW => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.asw_max ?? -1;
		public int CurASW => this.Level == 1 ? this.MinASW
			: this.Level == 99 ? this.MaxASW
			: this.MinASW == -1 || this.MaxASW == -1 ? -1
			: (int)Math.Floor((double)(this.MaxASW - this.MinASW) * this.Level / 99 + this.MinASW);

		///////////////////

		public ShipSpeed AbyssalSpeed { get; set; }
		public ShipSpeed Speed => ShipSpeedConverter.FromInt32(this.Ship?.RawData.api_soku ?? 0);
		public string AbyssalSpeedText
			=> this.AbyssalSpeed == ShipSpeed.Immovable ? "육상기지"
				: this.AbyssalSpeed == ShipSpeed.Slow ? "저속"
				: this.AbyssalSpeed == ShipSpeed.Fast ? "고속"
				: this.AbyssalSpeed == ShipSpeed.Faster ? "고속+"
				: this.AbyssalSpeed == ShipSpeed.Fastest ? "초고속"
				: "???";
		public string SpeedText
			=> this.Speed == ShipSpeed.Immovable ? "육상기지"
				: this.Speed == ShipSpeed.Slow ? "저속"
				: this.Speed == ShipSpeed.Fast ? "고속"
				: this.Speed == ShipSpeed.Faster ? "고속+"
				: this.Speed == ShipSpeed.Fastest ? "초고속"
				: "???";

		public int AbyssalRange { get; set; }
		public int Range => this.Ship?.RawData.api_leng ?? -1;
		public string AbyssalRangeText
			=> this.AbyssalRange == 1 ? "단거리"
				: this.AbyssalRange == 2 ? "중거리"
				: this.AbyssalRange == 3 ? "장거리"
				: this.AbyssalRange == 4 ? "초장거리"
				: "???";
		public string RangeText
			=> this.Range == 1 ? "단거리"
				: this.Range == 2 ? "중거리"
				: this.Range == 3 ? "장거리"
				: this.Range == 4 ? "초장거리"
				: "???";

		public int AbyssalLOS { get; set; }
		public int MinLOS => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.los ?? -1;
		public int MaxLOS => DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0))?.stat.los_max ?? -1;
		public int CurLOS => this.Level == 1 ? this.MinLOS
			: this.Level == 99 ? this.MaxLOS
			: this.MinLOS == -1 || this.MaxLOS == -1 ? -1
			: (int)Math.Floor((double)(this.MaxLOS - this.MinLOS) * this.Level / 99 + this.MinLOS);

		public int AbyssalLuck { get; set; }
		public int MinLuck => this.Ship?.RawData.api_luck?[0] ?? -1;
		public int MaxLuck => this.Ship?.RawData.api_luck?[1] ?? -1;
		public int CurLuck => this.MinLuck == -1 ? -1
			: this.MinLuck + (this.Marriaged ? 3 : 0);

		///////////////////

		public int MaxFuel => this.Ship?.RawData.api_fuel_max ?? -1;
		public int MaxAmmo => this.Ship?.RawData.api_bull_max ?? -1;

		public string CurMaxFuel => this.Marriaged
			? Math.Floor(this.MaxFuel * 0.85) + " (-15%)"
			: this.MaxFuel.ToString();
		public string CurMaxAmmo => this.Marriaged
			? Math.Floor(this.MaxAmmo * 0.85) + " (-15%)"
			: this.MaxAmmo.ToString();

		private void UpdateAllValues()
		{
			this.RaisePropertyChanged(nameof(this.MarriageHP));
			this.RaisePropertyChanged(nameof(this.MarriageHPResult));

			this.RaisePropertyChanged(nameof(this.HP));
			this.RaisePropertyChanged(nameof(this.CurHP));

			this.RaisePropertyChanged(nameof(this.MinArmor));
			this.RaisePropertyChanged(nameof(this.MaxArmor));
			this.RaisePropertyChanged(nameof(this.CurArmor));

			this.RaisePropertyChanged(nameof(this.MinEvasion));
			this.RaisePropertyChanged(nameof(this.MaxEvasion));
			this.RaisePropertyChanged(nameof(this.CurEvasion));

			this.RaisePropertyChanged(nameof(this.MinFire));
			this.RaisePropertyChanged(nameof(this.MaxFire));
			this.RaisePropertyChanged(nameof(this.CurFire));

			this.RaisePropertyChanged(nameof(this.MinTorpedo));
			this.RaisePropertyChanged(nameof(this.MaxTorpedo));
			this.RaisePropertyChanged(nameof(this.CurTorpedo));

			this.RaisePropertyChanged(nameof(this.MinAA));
			this.RaisePropertyChanged(nameof(this.MaxAA));
			this.RaisePropertyChanged(nameof(this.CurAA));

			this.RaisePropertyChanged(nameof(this.MinASW));
			this.RaisePropertyChanged(nameof(this.MaxASW));
			this.RaisePropertyChanged(nameof(this.CurASW));

			this.RaisePropertyChanged(nameof(this.MinLOS));
			this.RaisePropertyChanged(nameof(this.MaxLOS));
			this.RaisePropertyChanged(nameof(this.CurLOS));

			this.RaisePropertyChanged(nameof(this.MinLuck));
			this.RaisePropertyChanged(nameof(this.MaxLuck));
			this.RaisePropertyChanged(nameof(this.CurLuck));

			this.RaisePropertyChanged(nameof(this.MaxFuel));
			this.RaisePropertyChanged(nameof(this.CurMaxFuel));

			this.RaisePropertyChanged(nameof(this.MaxAmmo));
			this.RaisePropertyChanged(nameof(this.CurMaxAmmo));
		}
		#endregion

		// Equipment
		public IList<SlotInfo> Slots
		{
			get
			{
				var shipId = (this.Ship?.Id ?? 0);
				var slotCount = this.Ship?.SlotCount ?? 0;
				var list = new List<SlotInfo>();

				if (shipId > 1500) // Is abyssal?
				{
					var info = DictionaryViewModel.abyssalShipInfos.FirstOrDefault(x => x.id == shipId);
					if (info == null)
					{
						while (list.Count < slotCount)
							list.Add(new SlotInfo(null, true, this.Ship?.Slots?[list.Count] ?? -1));

						while (list.Count < 4) list.Add(new SlotInfo(null, false, 0));
						return list;
					}

					list = info.equips
						.Where(x => x > 0)
						.Select((x, z) =>
						{
							var master = KanColleClient.Current.Master;
							var item = master.SlotItems.FirstOrDefault(y => y.Value.Id == x).Value;
							if (item == null) return new SlotInfo(SlotItemInfo.Dummy, true, info.carrys[z]);

							return new SlotInfo(item, true, info.carrys[z]);
						})
						.ToList();

					while (list.Count < slotCount)
						list.Add(new SlotInfo(SlotItemInfo.Dummy, true, info.carrys[list.Count]));
				}
				else
				{
					var info = DictionaryViewModel.shipInfos.FirstOrDefault(x => x.id == shipId);
					if (info == null)
					{
						while (list.Count < slotCount)
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
						.Select((x, z) =>
						{
							int id;
							if (!int.TryParse(x, out id)) return new SlotInfo(SlotItemInfo.Dummy, true, this.Ship?.Slots[z] ?? -1);
							var master = KanColleClient.Current.Master;
							var item = master.SlotItems.FirstOrDefault(y => y.Value.Id == id).Value;
							if (item == null) return new SlotInfo(SlotItemInfo.Dummy, true, this.Ship?.Slots[z] ?? -1);

							return new SlotInfo(item, true, this.Ship?.Slots[z] ?? -1);
						})
						.ToList();

					while (list.Count < slotCount)
						list.Add(new SlotInfo(SlotItemInfo.Dummy, true, this.Ship?.Slots[list.Count] ?? -1));
				}

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

		#region Upgrade Information
		public IEnumerable<UpgradeInfo> Upgrades { get; set; }
		#endregion

		public ShipItemViewModel(DictionaryShipViewModel parent, KanColleWrapper.Models.ShipInfo ShipData)
		{
			this.Parent = parent;

			this.Ship = ShipData;
			this._Name = this.Ship?.Name ?? "???";
			this._Id = this.Ship?.Id.ToString() ?? "???";
			this._ShipType = this.Ship?.ShipType.Name ?? "???";

			if (this.IsAbyssal)
			{
				var info = DictionaryViewModel.abyssalShipInfos.FirstOrDefault(x => x.id == (this.Ship?.Id ?? 0));
				if (info == null)
				{
					this.AbyssalHP = -1;
					this.AbyssalArmor = -1;
					this.AbyssalEvasion = -1;
					this.AbyssalCarry = -1;
					this.AbyssalFire = -1;
					this.AbyssalTorpedo = -1;
					this.AbyssalAA = -1;
					this.AbyssalASW = -1;
					this.AbyssalSpeed = (ShipSpeed)(-1);
					this.AbyssalRange = -1;
					this.AbyssalLOS = -1;
					this.AbyssalLuck = -1;
				}
				else
				{
					this.AbyssalHP = info.hp;
					this.AbyssalArmor = info.armor;
					this.AbyssalEvasion = info.evasion;
					this.AbyssalCarry = info.carry;

					this.AbyssalFire = info.fire;
					this.AbyssalTorpedo = info.torpedo;
					this.AbyssalAA = info.aa;
					this.AbyssalASW = info.asw;

					this.AbyssalSpeed = ShipSpeedConverter.FromInt32(info.speed);
					this.AbyssalRange = info.range;
					this.AbyssalLOS = info.los;
					this.AbyssalLuck = info.luck;
				}
			}

			if (this.Ship == null) this.Upgrades = new UpgradeInfo[0];
			else
			{
				// Find root
				var master = KanColleClient.Current.Master;
				var root = this.Ship;

				while (master.Ships.Any(x => x.Value.RawData.api_aftershipid == root.Id.ToString()))
					root = master.Ships.FirstOrDefault(x => x.Value.RawData.api_aftershipid == root.Id.ToString()).Value;

				List<UpgradeInfo> list = new List<UpgradeInfo>();
				var reqLv = 1;

				while (!list.Any(x => x.Id == root.Id))
				{
					bool NeedPaper, NeedCatapult;
					NeedPaper = master.RawData.api_mst_shipupgrade
						.FirstOrDefault(x => x.api_id == root.Id)
						?.api_drawing_count > 0;
					NeedCatapult = master.RawData.api_mst_shipupgrade
						.FirstOrDefault(x => x.api_id == root.Id)
						?.api_catapult_count > 0;

					list.Add(new UpgradeInfo(this, root, reqLv, NeedPaper, NeedCatapult));
					if (!root.RemodelingExists) break;

					int after;
					if (!int.TryParse(root.RawData.api_aftershipid, out after)) break;
					if (after == 0) break;

					reqLv = root.NextRemodelingLevel ?? 1;
					root = master.Ships.FirstOrDefault(x => x.Value.Id == after).Value;
				}
				this.Upgrades = list.ToArray();
			}
		}
		public void SetShip(int ShipId)
		{
			this.Parent.SelectedShip = this.Parent.ShipList.FirstOrDefault(x => x.Ship?.Id == ShipId) ?? this;
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
	public class UpgradeInfo
	{
		private ShipItemViewModel Parent { get; }

		public int Id { get; }
		public int RequiredLevel { get; }
		public string Name { get; }
		public string Requires { get; }

		public string BadgeImage =>
			string.Format(
				"http://wolfgangkurz.github.io/KanColleAssets/ships/{0}/1.png",
				DictionaryViewModel.shipGraphics.FirstOrDefault(x => x.api_id == this.Id)?.api_filename ?? "none"
			);

		public UpgradeInfo(ShipItemViewModel parent, KanColleWrapper.Models.ShipInfo ShipData, int Level, bool NeedPaper = false, bool NeedCatapult = false)
		{
			this.Parent = parent;

			if (ShipData == null) return;

			this.Id = ShipData.Id;
			this.RequiredLevel = Level;
			this.Name = ShipData?.Name ?? "???";

			this.Requires = string.Join(
				", ",
				new string[]
				{
					NeedPaper ? "개장설계도" : "",
					NeedCatapult ? "캐터펄트" : ""
				}.Where(x => x.Length > 0).ToArray()
			);
		}

		public void SetShip(int ShipId)
			=> this.Parent.SetShip(ShipId);
	}
	#endregion

	#region For Equipment Dictionary Tab
	public class DictionaryEquipmentViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return "장비"; }
			protected set { throw new NotImplementedException(); }
		}

		#region Tab (List)
		public IList<EquipmentItemViewModel> EquipmentList { get; set; }

		private EquipmentItemViewModel _SelectedEquipment;
		public EquipmentItemViewModel SelectedEquipment
		{
			get { return this._SelectedEquipment; }
			set
			{
				if (this._SelectedEquipment != value)
				{
					this._SelectedEquipment = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		private string _SearchText { get; set; }
		public string SearchText
		{
			get { return this._SearchText; }
			set
			{
				if (this._SearchText != value)
				{
					this._SearchText = value;
					this.RaisePropertyChanged();

					this.UpdateList();
				}
			}
		}

		public DictionaryEquipmentViewModel()
		{
			UpdateList();
		}

		private void UpdateList()
		{
			this.EquipmentList = KanColleClient.Current.Master.SlotItems
				.OrderBy(x => x.Value.Id)
				.Where(x => x.Value.Name.IndexOf(this.SearchText ?? "", StringComparison.CurrentCultureIgnoreCase) >= 0)
				.Select(x => new EquipmentItemViewModel(x.Value))
				.ToArray();

			this.RaisePropertyChanged(nameof(this.EquipmentList));
		}
	}

	public class EquipmentItemViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return _Name; }
			protected set { throw new NotImplementedException(); }
		}
		public string Id => this._Id;
		public string ItemType => this._ItemType;

		private string _Name { get; set; }
		private string _Id { get; set; }
		private string _ItemType { get; set; }

		public SlotItemInfo Equipment { get; }

		public string CardImage =>
			string.Format(
				"http://wolfgangkurz.github.io/KanColleAssets/slotitem/card/{0:D3}.png",
				this.Equipment?.Id ?? 0
			);

		#region Status
		public int HP => this.Equipment?.RawData.api_taik ?? -1;
		public int Armor => this.Equipment?.RawData.api_souk ?? -1;
		public int Evasion => this.Equipment?.RawData.api_houk ?? -1;
		public int HitChance => this.Equipment?.RawData.api_houm ?? -1;

		public string EvasionName
			=> this.Equipment?.Type == SlotItemType.局地戦闘機
				? "영격" : "회피";
		public string EvasionIcon
			=> this.Equipment?.Type == SlotItemType.局地戦闘機
				? "http://wolfgangkurz.github.io/KanColleAssets/stats/carry.png"
				: "http://wolfgangkurz.github.io/KanColleAssets/stats/evasion.png";

		public string HitChanceName
			=> this.Equipment?.Type == SlotItemType.局地戦闘機
				? "대폭" : "명중";
		public string HitChanceIcon
			=> this.Equipment?.Type == SlotItemType.局地戦闘機
				? "http://wolfgangkurz.github.io/KanColleAssets/stats/bomb.png"
				: "http://wolfgangkurz.github.io/KanColleAssets/stats/hit.png";

		///////////////////

		public int Fire => this.Equipment?.RawData.api_houg ?? -1;
		public int Torpedo => this.Equipment?.RawData.api_raig ?? -1;
		public int AA => this.Equipment?.RawData.api_tyku ?? -1;
		public int ASW => this.Equipment?.RawData.api_tais ?? -1;

		///////////////////

		public int Bombing => this.Equipment?.RawData.api_baku ?? -1;
		public int TorpedoAircraft => this.Equipment?.RawData.api_taik ?? -1;
		public int AircraftCost
		{
			get
			{
				var cost = this.Equipment?.RawData.api_cost ?? -1;
				if (cost == -1) return cost;

				var type = this.Equipment?.Type ?? SlotItemType.None;
				if (type == SlotItemType.None) return cost;

				switch (type)
				{
					case SlotItemType.艦上偵察機:
					case SlotItemType.水上偵察機:
					case SlotItemType.噴式偵察機:
					case SlotItemType.大型飛行艇:
						return cost * 4;

					default:
						return cost * 12;
				}
		}
		}
		public int AircraftDistance => this.Equipment?.RawData.api_distance ?? -1;

		///////////////////

		public ShipSpeed Speed => ShipSpeedConverter.FromInt32(this.Equipment?.RawData.api_soku ?? 0);
		public string SpeedText
			=> this.Speed == ShipSpeed.Immovable ? "-"
				: this.Speed == ShipSpeed.Slow ? "저속"
				: this.Speed == ShipSpeed.Fast ? "고속"
				: this.Speed == ShipSpeed.Faster ? "고속+"
				: this.Speed == ShipSpeed.Fastest ? "초고속"
				: "???";

		public int Range => this.Equipment?.RawData.api_leng ?? -1;
		public string RangeText
			=> this.Range == 0 ? "-"
				: this.Range == 1 ? "단거리"
				: this.Range == 2 ? "중거리"
				: this.Range == 3 ? "장거리"
				: this.Range == 4 ? "초장거리"
				: "???";

		public int LOS => this.Equipment?.RawData.api_saku ?? -1;
		public int Luck => this.Equipment?.RawData.api_luck ?? -1;
		#endregion

		#region Modernize & Destroy
		public int DestroyFuel => this.Equipment?.RawData.api_broken?[0] ?? 0;
		public int DestroyAmmo => this.Equipment?.RawData.api_broken?[1] ?? 0;
		public int DestroySteel => this.Equipment?.RawData.api_broken?[2] ?? 0;
		public int DestroyBauxite => this.Equipment?.RawData.api_broken?[3] ?? 0;
		#endregion

		public EquipmentItemViewModel(SlotItemInfo EquipData)
		{
			this.Equipment = EquipData;
			this._Name = this.Equipment?.Name ?? "???";
			this._Id = this.Equipment?.Id.ToString() ?? "???";
			this._ItemType = this.Equipment?.TypeName ?? "???";
		}
	}
	#endregion

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
	public class ships_abyssal_info
	{
		public int id { get; set; }

		public int hp { get; set; }
		public int armor { get; set; }
		public int evasion { get; set; }
		public int carry { get; set; }

		public int fire { get; set; }
		public int torpedo { get; set; }
		public int aa { get; set; }
		public int asw { get; set; }

		public int speed { get; set; }
		public int range { get; set; }
		public int los { get; set; }
		public int luck { get; set; }

		public int slots { get; set; }
		public int[] equips { get; set; }
		public int[] carrys { get; set; }
	}
	// ReSharper restore InconsistentNaming
	#endregion
}
