using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.Commands;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public abstract class ShipCatalogFilter : NotificationObject
	{
		private readonly Action action;

		public abstract bool Predicate(Ship ship);

		protected ShipCatalogFilter(Action updateAction)
		{
			this.action = updateAction;
		}

		protected void Update()
		{
			this.action?.Invoke();
		}
	}

	public class ShipLevelFilter : ShipCatalogFilter
	{
		#region MinLevel 변경 통지 프로퍼티

		private string _MinLevel;

		public string MinLevel
		{
			get { return this._MinLevel; }
			set
			{
				if (this._MinLevel != value)
				{
					this._MinLevel = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region MaxLevel 변경 통지 프로퍼티

		private string _MaxLevel;

		public string MaxLevel
		{
			get { return this._MaxLevel; }
			set
			{
				if (this._MaxLevel != value)
				{
					this._MaxLevel = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public void SetLevelRange(string parameter)
		{
			string[] parts = parameter.Split('-');

			MinLevel = parts[0];
			MaxLevel = parts[1];
		}

		public ShipLevelFilter(Action updateAction)
			: base(updateAction)
		{
			this._MinLevel = "2";
			this._MaxLevel = "155";
		}

		public override bool Predicate(Ship ship)
		{
			int minlevel;
			int maxlevel;

			if (int.TryParse(_MinLevel, out minlevel) && int.TryParse(_MaxLevel, out maxlevel))
				if (ship.Level >= minlevel && ship.Level <= maxlevel)
					return true;

			return false;
		}
	}

	public class ShipLockFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		private bool _Both;

		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Locked 変更通知プロパティ

		private bool _Locked;

		public bool Locked
		{
			get { return this._Locked; }
			set
			{
				if (this._Locked != value)
				{
					this._Locked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Unlocked 変更通知プロパティ

		private bool _Unlocked;

		public bool Unlocked
		{
			get { return this._Unlocked; }
			set
			{
				if (this._Unlocked != value)
				{
					this._Unlocked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipLockFilter(Action updateAction)
			: base(updateAction)
		{
			this._Locked = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Locked && ship.IsLocked) return true;
			if (this.Unlocked && !ship.IsLocked) return true;

			return false;
		}
	}

	public class ShipSpeedFilter : ShipCatalogFilter
	{
		#region All 変更通知プロパティ
		private bool _All;
		public bool All
		{
			get { return this._All; }
			set
			{
				if (this._All != value)
				{
					this._All = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region SuperFast 変更通知プロパティ
		private bool _SuperFast;
		public bool SuperFast
		{
			get { return this._SuperFast; }
			set
			{
				if (this._SuperFast != value)
				{
					this._SuperFast = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region FastPlus 変更通知プロパティ
		private bool _FastPlus;
		public bool FastPlus
		{
			get { return this._FastPlus; }
			set
			{
				if (this._FastPlus != value)
				{
					this._FastPlus = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Fast 変更通知プロパティ
		private bool _Fast;
		public bool Fast
		{
			get { return this._Fast; }
			set
			{
				if (this._Fast != value)
				{
					this._Fast = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Low 変更通知プロパティ
		private bool _Low;
		public bool Low
		{
			get { return this._Low; }
			set
			{
				if (this._Low != value)
				{
					this._Low = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		public ShipSpeedFilter(Action updateAction)
			: base(updateAction)
		{
			this._All = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.All) return true;
			if (this.SuperFast && ship.Speed == ShipSpeed.SuperFast) return true;
			if (this.FastPlus && ship.Speed == ShipSpeed.FastPlus) return true;
			if (this.Fast && ship.Speed == ShipSpeed.Fast) return true;
			if (this.Low && ship.Speed == ShipSpeed.Low) return true;
			return false;
		}
	}

	public class ShipModernizeFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		private bool _Both;

		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region MaxModernized 変更通知プロパティ

		private bool _MaxModernized;

		public bool MaxModernized
		{
			get { return this._MaxModernized; }
			set
			{
				if (this._MaxModernized != value)
				{
					this._MaxModernized = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region NotMaxModernized 変更通知プロパティ

		private bool _NotMaxModernized;

		public bool NotMaxModernized
		{
			get { return this._NotMaxModernized; }
			set
			{
				if (this._NotMaxModernized != value)
				{
					this._NotMaxModernized = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipModernizeFilter(Action updateAction)
			: base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.MaxModernized && ship.IsMaxModernized) return true;
			if (this.NotMaxModernized && !ship.IsMaxModernized) return true;

			return false;
		}
	}

	public class ShipRemodelingFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		private bool _Both;

		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region AlreadyRemodeling 変更通知プロパティ

		private bool _AlreadyRemodeling;

		public bool AlreadyRemodeling
		{
			get { return this._AlreadyRemodeling; }
			set
			{
				if (this._AlreadyRemodeling != value)
				{
					this._AlreadyRemodeling = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region NotAlreadyRemodeling 変更通知プロパティ

		private bool _NotAlreadyRemodeling;

		public bool NotAlreadyRemodeling
		{
			get { return this._NotAlreadyRemodeling; }
			set
			{
				if (this._NotAlreadyRemodeling != value)
				{
					this._NotAlreadyRemodeling = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipRemodelingFilter(Action updateAction)
			: base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.AlreadyRemodeling && !ship.Info.NextRemodelingLevel.HasValue) return true;
			if (this.NotAlreadyRemodeling && ship.Info.NextRemodelingLevel.HasValue) return true;

			return false;
		}
	}

	public class ShipExpeditionFilter : ShipCatalogFilter
	{
		private HashSet<int> shipIds = new HashSet<int>();

		#region WithoutExpedition 変更通知プロパティ

		private bool _WithoutExpedition;

		public bool WithoutExpedition
		{
			get { return this._WithoutExpedition; }
			set
			{
				if (this._WithoutExpedition != value)
				{
					this._WithoutExpedition = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipExpeditionFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			return !this.WithoutExpedition || !this.shipIds.Contains(ship.Id);
		}

		public void SetFleets(MemberTable<Fleet> fleets)
		{
			if (fleets == null) return;

			this.shipIds = new HashSet<int>(fleets
				.Where(x => x.Value.Expedition.IsInExecution)
				.SelectMany(x => x.Value.Ships.Select(s => s.Id)));
		}
	}

	public class ShipSallyAreaFilter : ShipCatalogFilter
	{
		#region IsEnabled 変更通知プロパティ

		private bool _IsEnabled;

		public bool IsEnabled
		{
			get { return this._IsEnabled; }
			set
			{
				if (this._IsEnabled != value)
				{
					this._IsEnabled = value;
					this.ColumnWidth = value ? 65.0 : .0;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SallyAreas 変更通知プロパティ

		private SallyAreaFilterChild[] _SallyAreas;

		public SallyAreaFilterChild[] SallyAreas
		{
			get { return this._SallyAreas; }
			set
			{
				if (this._SallyAreas != value)
				{
					this._SallyAreas = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ColumnWidth 変更通知プロパティ

		private double _ColumnWidth;

		public double ColumnWidth
		{
			get { return this._ColumnWidth; }
			set
			{
				if (this._ColumnWidth != value)
				{
					this._ColumnWidth = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public ShipSallyAreaFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			// 出撃海域がない or 取得できなかったときは全艦通す
			return !this.IsEnabled || this.SallyAreas.Any(x => x.Predicate(ship));
		}

		public void SetSallyArea(SallyArea[] areas)
		{
			if (areas == null || areas.Length == 0)
			{
				this.IsEnabled = false;
				this.SallyAreas = new SallyAreaFilterChild[0];
			}
			else
			{
				this.SallyAreas = EnumerableEx
					.Return<SallyArea>(null)
					.Concat(areas)
					.Select(x => new SallyAreaFilterChild(x, this))
					.ToArray();
				this.IsEnabled = true;
			}

			this.Update();
		}

		public class SallyAreaFilterChild : ViewModel
		{
			private readonly SallyArea model;
			private readonly ShipSallyAreaFilter owner;

			#region Name 変更通知プロパティ

			private string _Name;

			public string Name
			{
				get { return this._Name; }
				set
				{
					if (this._Name != value)
					{
						this._Name = value;
						this.RaisePropertyChanged();
					}
				}
			}

			#endregion

			#region IsChecked 変更通知プロパティ

			private bool _IsChecked = true;

			public bool IsChecked
			{
				get { return this._IsChecked; }
				set
				{
					if (this._IsChecked != value)
					{
						this._IsChecked = value;
						this.RaisePropertyChanged();
						this.owner.Update();
					}
				}
			}

			#endregion

			public SallyAreaFilterChild(SallyArea area, ShipSallyAreaFilter owner)
			{
				this.model = area ?? SallyArea.Default;
				this.owner = owner;
				this.Name = area?.Name ?? "출격해역없음";
			}

			public bool Predicate(Ship ship)
			{
				return this.IsChecked && this.model.Area == ship.SallyArea;
			}
		}
	}

	public class ShipNameSearchFilter : ShipCatalogFilter
	{
		#region SearchCommand コマンド

		private ViewModelCommand _SearchCommand;

		public ViewModelCommand SearchCommand
		{
			get
			{
				if (this._SearchCommand == null)
				{
					this._SearchCommand = new ViewModelCommand(this.Update);
				}
				return this._SearchCommand;
			}
		}

		#endregion

		#region NameString 変更通知プロパティ

		private string _NameString;

		public string NameString
		{
			get { return this._NameString; }
			set
			{
				if (this._NameString != value)
				{
					this._NameString = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ShipNameSearchFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			if (this.NameString == null) return true;
			if (ship.Info.Name.Contains(this.NameString)) return true;
			return false;
		}
	}

	public class TimeToRepairFilter : ShipCatalogFilter
	{
		public TimeToRepairFilter(Action updateAction) : base(updateAction) { }

		public override bool Predicate(Ship ship)
		{
			if (ship.Situation.HasFlag(ShipSituation.Repair)) return false;
			if (ship.TimeToRepair != TimeSpan.Zero) return true;
			return false;
		}

	}

	public class ShipDamagedFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		private bool _Both;

		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Damaged 変更通知プロパティ

		private bool _Damaged;

		public bool Damaged
		{
			get { return this._Damaged; }
			set
			{
				if (this._Damaged != value)
				{
					this._Damaged = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Undamaged 変更通知プロパティ

		private bool _Undamaged;

		public bool Undamaged
		{
			get { return this._Undamaged; }
			set
			{
				if (this._Undamaged != value)
				{
					this._Undamaged = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipDamagedFilter(Action updateAction)
			: base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Damaged && ship.TimeToRepair != TimeSpan.Zero) return true;
			if (this.Undamaged && ship.TimeToRepair == TimeSpan.Zero) return true;

			return false;
		}
	}

	public class ShipConditionFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ

		private bool _Both;

		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Brilliant 変更通知プロパティ

		private bool _Brilliant;

		public bool Brilliant
		{
			get { return this._Brilliant; }
			set
			{
				if (this._Brilliant != value)
				{
					this._Brilliant = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Unbrilliant 変更通知プロパティ

		private bool _Unbrilliant;

		public bool Unbrilliant
		{
			get { return this._Unbrilliant; }
			set
			{
				if (this._Unbrilliant != value)
				{
					this._Unbrilliant = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipConditionFilter(Action updateAction)
			: base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Brilliant && ship.ConditionType == ConditionType.Brilliant) return true;
			if (this.Unbrilliant && ship.ConditionType >= ConditionType.Normal) return true;

			return false;
		}
	}

	public class ShipExSlotFilter : ShipCatalogFilter
	{
		#region Both 変更通知プロパティ
		private bool _Both;
		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Unequiped 変更通知プロパティ
		private bool _Unequiped;
		public bool Unequiped
		{
			get { return this._Unequiped; }
			set
			{
				if (this._Unequiped != value)
				{
					this._Unequiped = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Equiped 変更通知プロパティ
		private bool _Equiped;
		public bool Equiped
		{
			get { return this._Equiped; }
			set
			{
				if (this._Equiped != value)
				{
					this._Equiped = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		public ShipExSlotFilter(Action updateAction) : base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Unequiped && (!ship.ExSlotExists || !ship.ExSlot.Equipped)) return true;
			if (this.Equiped && ship.ExSlotExists && ship.ExSlot.Equipped) return true;

			return false;
		}
	}

	public class ShipFleetFilter : ShipCatalogFilter
	{
		#region AllFleet 변경 통지 프로퍼티
		private bool _AllFleet;

		public bool AllFleet
		{
			get { return this._AllFleet; }
			set
			{
				if (this._AllFleet != value)
				{
					this._AllFleet = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region FirstFleet 변경 통지 프로퍼티
		private bool _FirstFleet;

		public bool FirstFleet
		{
			get { return this._FirstFleet; }
			set
			{
				if (this._FirstFleet != value)
				{
					this._FirstFleet = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region SecondFleet 변경 통지 프로퍼티
		private bool _SecondFleet;

		public bool SecondFleet
		{
			get { return this._SecondFleet; }
			set
			{
				if (this._SecondFleet != value)
				{
					this._SecondFleet = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region ThirdFleet 변경 통지 프로퍼티
		private bool _ThirdFleet;

		public bool ThirdFleet
		{
			get { return this._ThirdFleet; }
			set
			{
				if (this._ThirdFleet != value)
				{
					this._ThirdFleet = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region FourthFleet 변경 통지 프로퍼티
		private bool _FourthFleet;

		public bool FourthFleet
		{
			get { return this._FourthFleet; }
			set
			{
				if (this._FourthFleet != value)
				{
					this._FourthFleet = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		public ShipFleetFilter(Action updateAction) : base(updateAction)
		{
			this._AllFleet = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.AllFleet) return true;
			if (this.FirstFleet && ship.FleetId == 1) return true;
			if (this.SecondFleet && ship.FleetId == 2) return true;
			if (this.ThirdFleet && ship.FleetId == 3) return true;
			if (this.FourthFleet && ship.FleetId == 4) return true;

			return false;
		}
	}
}
