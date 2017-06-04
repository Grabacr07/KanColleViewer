using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Mvvm;
using Livet;
using Livet.Messaging;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Grabacr07.KanColleViewer.Views.Catalogs;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using System.Runtime.Serialization.Json;
using kcsapi_mst_shipgraph = Grabacr07.KanColleWrapper.Models.Raw.kcsapi_mst_shipgraph;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class PresetFleetWindowViewModel : WindowViewModel
	{
		#region Fleets 변경 통지 프로퍼티
		private IReadOnlyCollection<PresetFleetData> _Fleets { get; set; }
		public IReadOnlyCollection<PresetFleetData> Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SelectedFleet 변경 통지 프로퍼티
		private PresetFleetData _SelectedFleet { get; set; }
		public PresetFleetData SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (this._SelectedFleet != value)
				{
					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		private string RecordPath => Path.Combine(
			Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
			"Record",
			"fleet_preset.json"
		);

		public PresetFleetWindowViewModel()
		{
			this.Title = "함대 프리셋";

			this.LoadFleets();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}

		public void LoadFleets()
		{
			var list = new List<PresetFleetData>();

			if (File.Exists(RecordPath))
			{
				var lines = File.ReadAllText(RecordPath)
					.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

				foreach (var line in lines)
				{
					var item = new PresetFleetData();
					item.Deserialize(line);
					list.Add(item);
				}
			}

			this.Fleets = list.ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}
		public void SaveFleets()
		{
			var datas = this.Fleets.Select(x => x.Serialize());

			File.WriteAllText(
				RecordPath,
				string.Join(Environment.NewLine, datas)
			);
		}

		public void ShowPresetAddWindow()
		{
			var catalog = new PresetFleetAddWindowViewModel(this);
			WindowService.Current.MainWindow.Transition(catalog, typeof(PresetFleetAddWindow));
		}
		public void ShowPresetDeleteWindow()
		{
			var fleet = this.SelectedFleet;
			if (fleet == null) return;

			var catalog = new PresetFleetDeleteWindowViewModel(this, fleet);
			WindowService.Current.MainWindow.Transition(catalog, typeof(PresetFleetDeleteWindow));
		}

		public void AddFleet(PresetFleetData fleet)
		{
			this.Fleets = this.Fleets
				.Concat(new PresetFleetData[] { fleet })
				.ToArray();

			if (this.SelectedFleet == null)
				this.SelectedFleet = this.Fleets.FirstOrDefault();

			SaveFleets();
		}
		public void DeleteFleet(PresetFleetData fleet)
		{
			this.Fleets = this.Fleets
				.Where(x => x != fleet)
				.ToArray();

			if (this.SelectedFleet == fleet)
				this.SelectedFleet = this.Fleets.FirstOrDefault();

			SaveFleets();
		}
	}

	#region Model Wrapper
	public class PresetFleetData : ViewModel
	{
		public PresetFleetModel Source { get; private set; }

		public string Name => this.Source?.Name;

		public string AirSuperiorityMinimum
			=> (
				this.Source?.Ships
					.Sum(x => x.GetAirSuperiorityPotential(AirSuperiorityCalculationOptions.Minimum))
					?? 0
			).ToString("##0");

		public string AirSuperiorityMaximum
			=> (
				this.Source?.Ships
					.Sum(x => x.GetAirSuperiorityPotential(AirSuperiorityCalculationOptions.Maximum))
					?? 0
			).ToString("##0");

		private ICalcViewRange logic = ViewRangeCalcLogic.Get(KanColleClient.Current.Settings.ViewRangeCalcType);
		public string ViewRange
			=> (this.Source?.Ships.GetViewRange() ?? 0)
				.ToString("##0.##");

		public PresetShipData[] Ships
			=> this.Source?.Ships
				.Select((x, y) =>
				{
					var z = new PresetShipData(x);
					z.Index = y + 1;
					return z;
				})
				.Where(x => x.Id > 0)
				.ToArray();


		public PresetFleetData()
		{
			this.Source = new PresetFleetModel
			{
				Name = "Untitled Fleet",
				Ships = new PresetShipModel[0]
			};
		}
		public PresetFleetData(string Name) : this()
		{
			this.Source.Name = Name;
		}
		public PresetFleetData(PresetFleetModel fleet)
		{
			this.Source = fleet;
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetFleetModel.Deserialize(Data);
	}
	public class PresetShipData : NotificationObject
	{
		public ShipInfo Ship =>
				KanColleClient.Current.Master.Ships
					.SingleOrDefault(x => x.Value.Id == this.Source.Id).Value
					?? null;

		public PresetShipModel Source { get; private set; }

		#region Index Property
		private int _Index { get; set; }
		public int Index {
			get { return this._Index; }
			set
			{
				if (this._Index != value)
				{
					this._Index = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public int Id => this.Source?.Id ?? 0;
		public string Name => this.Ship?.Name ?? "？？？";
		public string TypeName => this.Ship?.ShipType?.Name ?? "？？？";
		public int Level => this.Source?.Level ?? 0;

		public string Speed
			=> (ShipSpeed?)this.Source?.Speed == ShipSpeed.Immovable ? "육상기지"
				: (ShipSpeed?)this.Source?.Speed == ShipSpeed.Slow ? "저속"
				: (ShipSpeed?)this.Source?.Speed == ShipSpeed.Fast ? "고속"
				: (ShipSpeed?)this.Source?.Speed == ShipSpeed.Faster ? "고속+"
				: (ShipSpeed?)this.Source?.Speed == ShipSpeed.Fastest ? "초고속"
				: "？？？";

		public string Range
			=> this.Source?.Range == 0 ? "없음"
				: this.Source?.Range == 1 ? "단거리"
				: this.Source?.Range == 2 ? "중거리"
				: this.Source?.Range == 3 ? "장거리"
				: this.Source?.Range == 4 ? "초장거리"
				: "？？？";

		public PresetSlotData[] Slots
			=> this.Source.Slots?
				.Select(x => new PresetSlotData(x))
				.Take(Ship.SlotCount)
				.Select((x, y) =>
				{
					x.Carry = Ship.Slots[y];
					return x;
				})
				.ToArray();
		public PresetSlotData ExSlot
			=> new PresetSlotData(this.Source?.ExSlot);

		public PresetShipData()
		{
			this.Source = new PresetShipModel
			{
				Id = -1,
				Slots = new PresetSlotModel[0],
				ExSlot = null
			};
		}
		public PresetShipData(Ship ship)
		{
			this.Source = new PresetShipModel
			{
				Id = ship.Info.Id,
				Level = ship.Level,

				Slots = ship.Slots
					.Select(x => new PresetSlotModel
					{
						Id = x.Item.Info.Id,
						Level = x.Item.Level,
						Proficiency = x.Item.Proficiency
					})
					.Take(ship.Info.SlotCount)
					.ToArray(),

				ExSlot = ship.ExSlot != null
					? new PresetSlotModel
					{
						Id = ship.ExSlot.Item.Info.Id,
						Level = ship.ExSlot.Item.Level,
						Proficiency = ship.ExSlot.Item.Proficiency
					}
					: null,

				HP = ship.HP.Maximum,
				Armor = ship.Armer.Current,
				Evasion = ship.RawData.api_kaihi[0],
				Carries = ship.Info.RawData.api_maxeq.Sum(),
				Speed = (int)ship.Speed,
				Range = ship.RawData.api_leng,

				Firepower = ship.Firepower.Current,
				Torpedo = ship.Torpedo.Current,
				AA = ship.AA.Current,
				ASW = ship.ASW.Current,
				LOS = ship.ViewRange,
				Luck = ship.Luck.Current
			};
		}
		public PresetShipData(PresetShipModel ship)
		{
			this.Source = ship;
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetShipModel.Deserialize(Data);
	}
	public class PresetSlotData : NotificationObject
	{
		public SlotItemInfo Item =>
			KanColleClient.Current.Master.SlotItems
				.SingleOrDefault(x => x.Value.Id == this.Source?.Id).Value
				?? null;

		public PresetSlotModel Source { get; private set; }

		public int Id => this.Source?.Id ?? 0;
		public int Carry { get; set; }

		public string Name => this.Item?.Name ?? "？？？";
		public int Level => this.Source?.Level ?? 0;
		public int Proficiency => this.Source?.Proficiency ?? 0;

		public string Description =>
			this.Name
			+ (this.Level > 0 ? (this.Level == 10 ? " ★max" : $" ★+{this.Level}") : "")
			+ (this.Proficiency > 0 ? $" +{this.Proficiency}" : "")
			+ Environment.NewLine + Environment.NewLine
			+ this.Item?.ToolTipData ?? "";

		public PresetSlotData()
		{
			this.Source = new PresetSlotModel
			{
				Id = 0,
				Level = 0,
				Proficiency = 0
			};
		}
		public PresetSlotData(SlotItem item)
		{
			this.Source = new PresetSlotModel
			{
				Id = item.Info.Id,
				Level = item.Level,
				Proficiency = item.Proficiency
			};
		}
		public PresetSlotData(PresetSlotModel item)
		{
			this.Source = item;
		}

		public string Serialize()
			=> this.Source?.Serialize();

		public void Deserialize(string Data)
			=> this.Source = PresetSlotModel.Deserialize(Data);
	}
	#endregion

	#region Models
	public class PresetFleetModel
	{
		public string Name { get; set; }
		public PresetShipModel[] Ships { get; set; }

		public string Serialize()
		{
			return string.Format(
				"{{\"Name\":\"{0}\",\"Ships\":[{1}]}}",
				this.Name.Replace("\"", "\\\""),
				string.Join(",", this.Ships.Select(x => x.Serialize()))
			);
		}
		public static PresetFleetModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetFleetModel>(Data);
	}
	public class PresetShipModel
	{
		public int Id { get; set; }
		public int Level { get; set; }

		public int HP { get; set; }
		public int Armor { get; set; }
		public int Evasion { get; set; }
		public int Carries { get; set; }
		public int Speed { get; set; }
		public int Range { get; set; }

		public int Firepower { get; set; }
		public int Torpedo { get; set; }
		public int AA { get; set; }
		public int ASW { get; set; }
		public int LOS { get; set; }
		public int Luck { get; set; }

		public PresetSlotModel[] Slots;
		public PresetSlotModel ExSlot;

		public string Serialize()
		{
			return string.Format(
				"{{\"Id\":{0},\"Level\":{1},\"HP\":{2},\"Armor\":{3},\"Evasion\":{4},\"Carries\":{5},\"Speed\":{6},\"Range\":{7},\"Firepower\":{8},"
				+ "\"Torpedo\":{9},\"AA\":{10},\"ASW\":{11},\"LOS\":{12},\"Luck\":{13},\"Slots\":[{14}],\"ExSlot\":{15}}}",

				this.Id,
				this.Level,

				this.HP,
				this.Armor,
				this.Evasion,
				this.Carries,
				this.Speed,
				this.Range,

				this.Firepower,
				this.Torpedo,
				this.AA,
				this.ASW,
				this.LOS,
				this.Luck,

				string.Join(",", this.Slots.Select(x => x.Serialize())),
				this.ExSlot.Serialize()
			);
		}
		public static PresetShipModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetShipModel>(Data);
	}
	public class PresetSlotModel
	{
		public int Id { get; set; }
		public int Level { get; set; }
		public int Proficiency { get; set; }

		public string Serialize()
		{
			return string.Format(
				"{{\"Id\":{0},\"Level\":{1},\"Proficiency\":{2}}}",
				this.Id,
				this.Level,
				this.Proficiency
			);
		}
		public static PresetSlotModel Deserialize(string Data)
			=> PresetUtil.ParseJson<PresetSlotModel>(Data);
	}
	#endregion

	internal static class PresetUtil
	{
		internal static IEnumerable<kcsapi_mst_shipgraph> ShipGraphics
			=> KanColleClient.Current.Master.RawData?.api_mst_shipgraph ?? new kcsapi_mst_shipgraph[0];

		public static T ParseJson<T>(string Json) where T : class
		{
			var bytes = Encoding.UTF8.GetBytes(Json);
			var serializer = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(bytes))
			{
				var rawResult = serializer.ReadObject(stream) as T;
				return rawResult;
			}
		}

		public static double GetAirSuperiorityPotential(this PresetShipModel ship, AirSuperiorityCalculationOptions option)
		{
			var slots = new PresetShipData(ship).Ship.RawData.api_maxeq;

			return ship.Slots
				.Select((x, y) =>
				{
					var calc = new PresetSlotData(x).Item?.Type.GetCalculator() ?? EmptyCalculator.Instance;

					if (slots[y] <= 0) return 0;
					if (!option.HasFlag(calc.Options)) return 0;

					return calc.GetAirSuperiority(x, slots[y], option);
				})
				.Sum();
		}
		private static AirSuperiorityCalculator GetCalculator(this SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.艦上戦闘機:
				case SlotItemType.水上戦闘機:
				case SlotItemType.噴式戦闘機:
					return new FighterCalculator();

				case SlotItemType.艦上攻撃機:
				case SlotItemType.艦上爆撃機:
				case SlotItemType.噴式攻撃機:
				case SlotItemType.噴式戦闘爆撃機:
					// case SlotItemType.噴式偵察機: ??
					return new AttackerCalculator();

				case SlotItemType.水上爆撃機:
					return new SeaplaneBomberCalculator();

				default:
					return EmptyCalculator.Instance;
			}
		}
		private abstract class AirSuperiorityCalculator
		{
			public abstract AirSuperiorityCalculationOptions Options { get; }

			public int GetAirSuperiority(PresetSlotModel slotItem, int onslot, AirSuperiorityCalculationOptions options)
			{
				// 装備の対空値とスロットの搭載数による制空値
				var airSuperiority = this.GetAirSuperiority(slotItem, onslot);

				// 装備の熟練度による制空値ボーナス
				airSuperiority += this.GetProficiencyBonus(slotItem, options);

				return (int)airSuperiority;
			}

			protected virtual double GetAirSuperiority(PresetSlotModel slotItem, int onslot)
			{
				return new PresetSlotData(slotItem).Item.AA * Math.Sqrt(onslot);
			}

			protected abstract double GetProficiencyBonus(PresetSlotModel slotItem, AirSuperiorityCalculationOptions options);
		}
		#region AirSuperiority Calculators
		private class FighterCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Fighter;

			protected override double GetAirSuperiority(PresetSlotModel slotItem, int onslot)
			{
				// 装備改修による対空値加算 (★ x 0.2)
				return (new PresetSlotData(slotItem).Item.AA + slotItem.Level * 0.2) * Math.Sqrt(onslot);
			}

			protected override double GetProficiencyBonus(PresetSlotModel slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.FighterBonus;
			}
		}
		private class AttackerCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Attacker;

			protected override double GetProficiencyBonus(PresetSlotModel slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0);
			}
		}
		private class SeaplaneBomberCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.SeaplaneBomber;

			protected override double GetProficiencyBonus(PresetSlotModel slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.SeaplaneBomberBonus;
			}
		}
		private class EmptyCalculator : AirSuperiorityCalculator
		{
			public static EmptyCalculator Instance { get; } = new EmptyCalculator();

			public override AirSuperiorityCalculationOptions Options => ~AirSuperiorityCalculationOptions.Default;
			protected override double GetAirSuperiority(PresetSlotModel slotItem, int onslot) => .0;
			protected override double GetProficiencyBonus(PresetSlotModel slotItem, AirSuperiorityCalculationOptions options) => .0;

			private EmptyCalculator() { }
		}
		#endregion
		#region Proficiency Bonus
		private class Proficiency
		{
			private readonly int internalMinValue;
			private readonly int internalMaxValue;

			public int FighterBonus { get; }
			public int SeaplaneBomberBonus { get; }

			public Proficiency(int internalMin, int internalMax, int fighterBonus, int seaplaneBomberBonus)
			{
				this.internalMinValue = internalMin;
				this.internalMaxValue = internalMax;
				this.FighterBonus = fighterBonus;
				this.SeaplaneBomberBonus = seaplaneBomberBonus;
			}

			/// <summary>
			/// 内部熟練度値を取得します。
			/// </summary>
			public int GetInternalValue(AirSuperiorityCalculationOptions options)
			{
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMinValue)) return this.internalMinValue;
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMaxValue)) return this.internalMaxValue;
				return (this.internalMaxValue + this.internalMinValue) / 2; // <- めっちゃ適当
			}
		}
		private static readonly Dictionary<int, Proficiency> proficiencies = new Dictionary<int, Proficiency>()
		{
			{ 0, new Proficiency(0, 9, 0, 0) },
			{ 1, new Proficiency(10, 24, 0, 0) },
			{ 2, new Proficiency(25, 39, 2, 1) },
			{ 3, new Proficiency(40, 54, 5, 1) },
			{ 4, new Proficiency(55, 69, 9, 1) },
			{ 5, new Proficiency(70, 84, 14, 3) },
			{ 6, new Proficiency(85, 99, 14, 3) },
			{ 7, new Proficiency(100, 120, 22, 6) },
		};
		private static Proficiency GetProficiency(this PresetSlotModel slotItem) => proficiencies[Math.Max(Math.Min(slotItem.Proficiency, 7), 0)];
		#endregion

		public static double GetViewRange(this PresetShipModel[] ships)
		{
			var itemLOS = ships
				.SelectMany(x => x.Slots)
				.Sum(x => {
					var y = new PresetSlotData(x).Item;
					if (y == null) return 0;

					return (y.ViewRange + GetLevelCoefficient(y.Type, x.Level)) * GetTypeCoefficient(y.Type);
				});

			var shipLOS = ships
				.Select(x => x.LOS - x.Slots.Sum(y => new PresetSlotData(y).Item?.RawData.api_saku ?? 0))
				.Sum(x => Math.Sqrt(x));

			var admiralLOS = Math.Ceiling(KanColleClient.Current.Homeport.Admiral.Level * 0.4);
			var vacancyScore = (6 - ships.Count(x => x.Id > 0)) * 2;

			return itemLOS + shipLOS - admiralLOS + vacancyScore;
		}
		private static double GetLevelCoefficient(SlotItemType type, int Level)
		{
			switch (type)
			{
				case SlotItemType.水上偵察機:
					return Math.Sqrt(Level) * 1.2;

				case SlotItemType.小型電探:
				case SlotItemType.大型電探:
				case SlotItemType.大型電探_II:
					return Math.Sqrt(Level) * 1.25;

				default:
					return 0;
			}
		}
		private static double GetTypeCoefficient(SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.艦上戦闘機:
				case SlotItemType.艦上爆撃機:
				case SlotItemType.小型電探:
				case SlotItemType.大型電探:
				case SlotItemType.対潜哨戒機:
				case SlotItemType.探照灯:
				case SlotItemType.司令部施設:
				case SlotItemType.航空要員:
				case SlotItemType.水上艦要員:
				case SlotItemType.大型ソナー:
				case SlotItemType.大型飛行艇:
				case SlotItemType.大型探照灯:
				case SlotItemType.水上戦闘機:
				case SlotItemType.噴式戦闘機: // 未実装なのでﾃｷﾄｰ
				case SlotItemType.噴式戦闘爆撃機:
					return 0.6;

				case SlotItemType.艦上攻撃機:
				case SlotItemType.噴式攻撃機: // 未実装なのでﾃｷﾄｰ
					return 0.8;

				case SlotItemType.艦上偵察機:
				case SlotItemType.艦上偵察機_II:
					return 1.0;

				case SlotItemType.水上爆撃機:
					return 1.1;

				case SlotItemType.水上偵察機:
				case SlotItemType.噴式偵察機: // 未実装なのでﾃｷﾄｰ
					return 1.2;

				default:
					return .0;
			}
		}
	}
}
