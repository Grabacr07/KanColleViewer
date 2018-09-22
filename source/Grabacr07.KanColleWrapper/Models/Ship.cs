using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 母港に所属している艦娘を表します。
	/// </summary>
	public class Ship : RawDataWrapper<kcsapi_ship2>, IIdentifiable
	{
		private readonly Homeport homeport;

		/// <summary>
		/// この艦娘を識別する ID を取得します。
		/// </summary>
		public int Id => this.RawData.api_id;

		/// <summary>
		/// 艦娘の種類に基づく情報を取得します。
		/// </summary>
		public ShipInfo Info { get; private set; }

		public int SortNumber => this.RawData.api_sortno;

		/// <summary>
		/// 艦娘の現在のレベルを取得します。
		/// </summary>
		public int Level => this.RawData.api_lv;

		/// <summary>
		/// 艦娘がロックされているかどうかを示す値を取得します。
		/// </summary>
		public bool IsLocked => this.RawData.api_locked == 1;

		/// <summary>
		/// 艦娘の現在の累積経験値を取得します。
		/// </summary>
		public int Exp => this.RawData.api_exp.Get(0) ?? 0;

		/// <summary>
		/// この艦娘が次のレベルに上がるために必要な経験値を取得します。
		/// </summary>
		public int ExpForNextLevel => this.RawData.api_exp.Get(1) ?? 0;

		/// <summary>
		/// この艦娘の次の改造までに必要な経験値を取得します。
		/// </summary>
		public int ExpForNextRemodelingLevel => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, this.Info?.NextRemodelingLevel), 0);

		/// <summary>
		/// この艦娘とケッコンカッコカリするために必要な経験値を取得します。
		/// </summary>
		public int ExpForMarrige => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, 99), 0);

		/// <summary>
		/// この艦娘が最大Lvになるために必要な経験値を取得します。
		/// </summary>
		public int ExpForLevelMax => Experience.GetShipExpForSpecifiedLevel(this.Exp, 165);

		#region HP 変更通知プロパティ

		private LimitedValue _HP;

		/// <summary>
		/// 耐久値を取得します。
		/// </summary>
		public LimitedValue HP
		{
			get { return this._HP; }
			private set
			{
				this._HP = value;
				this.RaisePropertyChanged();

				if (value.IsHeavilyDamage())
				{
					this.Situation |= ShipSituation.HeavilyDamaged;
				}
				else
				{
					this.Situation &= ~ShipSituation.HeavilyDamaged;
				}
			}
		}

		#endregion

		#region Fuel 変更通知プロパティ

		private LimitedValue _Fuel;

		/// <summary>
		/// 燃料を取得します。
		/// </summary>
		public LimitedValue Fuel
		{
			get { return this._Fuel; }
			private set
			{
				this._Fuel = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Bull 変更通知プロパティ

		private LimitedValue _Bull;

		public LimitedValue Bull
		{
			get { return this._Bull; }
			private set
			{
				this._Bull = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Firepower 変更通知プロパティ

		private ModernizableStatus _Firepower;

		/// <summary>
		/// 火力ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Firepower
		{
			get { return this._Firepower; }
			private set
			{
				this._Firepower = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Torpedo 変更通知プロパティ

		private ModernizableStatus _Torpedo;

		/// <summary>
		/// 雷装ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Torpedo
		{
			get { return this._Torpedo; }
			private set
			{
				this._Torpedo = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region AA 変更通知プロパティ

		private ModernizableStatus _AA;

		/// <summary>
		/// 対空ステータス値を取得します。
		/// </summary>
		public ModernizableStatus AA
		{
			get { return this._AA; }
			private set
			{
				this._AA = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Armer 変更通知プロパティ

		private ModernizableStatus _Armer;

		/// <summary>
		/// 装甲ステータス値を取得します。
		/// </summary>
		public ModernizableStatus Armer
		{
			get { return this._Armer; }
			private set
			{
				this._Armer = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Luck 変更通知プロパティ

		private ModernizableStatus _Luck;

		/// <summary>
		/// 運のステータス値を取得します。
		/// </summary>
		public ModernizableStatus Luck
		{
			get { return this._Luck; }
			private set
			{
				this._Luck = value;
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Slots 変更通知プロパティ

		private ShipSlot[] _Slots;

		/// <summary>
		/// この艦娘の装備スロットを取得します。装備していないスロットには <see cref="ShipSlot.Equipped"/> が false のオブジェクトが割り当てられます (null を返しません)。
		/// </summary>
		public ShipSlot[] Slots
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

		#region EquippedSlots 変更通知プロパティ

		private ShipSlot[] _EquippedSlots;

		/// <summary>
		/// <see cref="Slots"/> と <see cref="ExSlot"/> のなかで <see cref="ShipSlot.Equipped"/> が true (空でないスロット) を列挙します。
		/// </summary>
		public ShipSlot[] EquippedItems
		{
			get { return this._EquippedSlots; }
			set
			{
				if (this._EquippedSlots != value)
				{
					this._EquippedSlots = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ExSlot 変更通知プロパティ

		private ShipSlot _ExSlot;

		public ShipSlot ExSlot
		{
			get { return this._ExSlot; }
			set
			{
				if (this._ExSlot != value)
				{
					this._ExSlot = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TimeToRepair 変更通知プロパティ

		private TimeSpan _TimeToRepair;

		public TimeSpan TimeToRepair
		{
			get { return this._TimeToRepair; }
			set
			{
				if (this._TimeToRepair != value)
				{
					this._TimeToRepair = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// 装備によるボーナスを含めた艦の速力を取得します。
		/// </summary>
		public ShipSpeed Speed => ShipSpeedConverter.FromInt32(this.RawData.api_soku);

		/// <summary>
		/// 装備によるボーナスを含めた索敵ステータス値を取得します。
		/// </summary>
		public int ViewRange => this.RawData.api_sakuteki.Get(0) ?? 0;

		public int ASW => this.RawData.api_taisen.Get(0) ?? 0;

		/// <summary>
		/// 火力・雷装・対空・装甲のすべてのステータス値が最大値に達しているかどうかを示す値を取得します。
		/// </summary>
		public bool IsMaxModernized => this.Firepower.IsMax && this.Torpedo.IsMax && this.AA.IsMax && this.Armer.IsMax;

		/// <summary>
		/// 現在のコンディション値を取得します。
		/// </summary>
		public int Condition => this.RawData.api_cond;

		/// <summary>
		/// コンディションの種類を示す <see cref="ConditionType" /> 値を取得します。
		/// </summary>
		public ConditionType ConditionType => ConditionTypeHelper.ToConditionType(this.RawData.api_cond);

		/// <summary>
		/// この艦が出撃した海域を識別する整数値を取得します。
		/// </summary>
		public int SallyArea => this.RawData.api_sally_area;

		#region Status 変更通知プロパティ

		private ShipSituation situation;

		public ShipSituation Situation
		{
			get { return this.situation; }
			set
			{
				if (this.situation != value)
				{
					this.situation = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		internal Ship(Homeport parent, kcsapi_ship2 rawData)
			: base(rawData)
		{
			this.homeport = parent;
			this.Update(rawData);
		}

		internal void Update(kcsapi_ship2 rawData)
		{
			this.UpdateRawData(rawData);

			this.Info = KanColleClient.Current.Master.Ships[rawData.api_ship_id] ?? ShipInfo.Dummy;
			this.HP = new LimitedValue(this.RawData.api_nowhp, this.RawData.api_maxhp, 0);
			this.Fuel = new LimitedValue(this.RawData.api_fuel, this.Info.RawData.api_fuel_max, 0);
			this.Bull = new LimitedValue(this.RawData.api_bull, this.Info.RawData.api_bull_max, 0);

			if (this.RawData.api_kyouka.Length >= 5)
			{
				this.Firepower = new ModernizableStatus(this.Info.RawData.api_houg, this.RawData.api_kyouka[0]);
				this.Torpedo = new ModernizableStatus(this.Info.RawData.api_raig, this.RawData.api_kyouka[1]);
				this.AA = new ModernizableStatus(this.Info.RawData.api_tyku, this.RawData.api_kyouka[2]);
				this.Armer = new ModernizableStatus(this.Info.RawData.api_souk, this.RawData.api_kyouka[3]);
				this.Luck = new ModernizableStatus(this.Info.RawData.api_luck, this.RawData.api_kyouka[4]);
			}

			this.TimeToRepair = TimeSpan.FromMilliseconds(this.RawData.api_ndock_time);

			this.UpdateSlots();
		}

		public void UpdateSlots()
		{
			this.Slots = this.RawData.api_slot
				.Select(id => this.homeport.Itemyard.SlotItems[id])
				.Select((t, i) => new ShipSlot(t, this.Info.RawData.api_maxeq.Get(i) ?? 0, this.RawData.api_onslot.Get(i) ?? 0))
				.ToArray();
			this.ExSlot = new ShipSlot(this.homeport.Itemyard.SlotItems[this.RawData.api_slot_ex], 0, 0);
			this.EquippedItems = this.EnumerateAllEquippedItems().ToArray();

			if (this.EquippedItems.Any(x => x.Item.Info.Type == SlotItemType.応急修理要員))
			{
				this.Situation |= ShipSituation.DamageControlled;
			}
			else
			{
				this.Situation &= ~ShipSituation.DamageControlled;
			}
		}


		internal void Charge(int fuel, int bull, int[] onslot)
		{
			this.Fuel = this.Fuel.Update(fuel);
			this.Bull = this.Bull.Update(bull);
			for (var i = 0; i < this.Slots.Length; i++) this.Slots[i].Current = onslot.Get(i) ?? 0;
		}

		internal void Repair()
		{
			var max = this.HP.Maximum;
			this.HP = this.HP.Update(max);
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Info.Name}\", ShipType = \"{this.Info.ShipType.Name}\", Level = {this.Level}";
		}


		private IEnumerable<ShipSlot> EnumerateAllEquippedItems()
		{
			foreach (var slot in this.Slots.Where(x => x.Equipped)) yield return slot;
			if (this.ExSlot.Equipped) yield return this.ExSlot;
		}
	}
}
