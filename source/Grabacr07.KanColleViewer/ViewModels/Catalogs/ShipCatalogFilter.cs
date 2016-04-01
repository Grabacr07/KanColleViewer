using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

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

		#region Level1 変更通知プロパティ

		private bool _Level1;

		public bool Level1
		{
			get { return this._Level1; }
			set
			{
				if (this._Level1 != value)
				{
					this._Level1 = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region Level2OrMore 変更通知プロパティ

		private bool _Level2OrMore;

		public bool Level2OrMore
		{
			get { return this._Level2OrMore; }
			set
			{
				if (this._Level2OrMore != value)
				{
					this._Level2OrMore = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		public ShipLevelFilter(Action updateAction)
			: base(updateAction)
		{
			this._Level2OrMore = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Level2OrMore && ship.Level >= 2) return true;
			if (this.Level1 && ship.Level == 1) return true;

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
			this._Both = true;
		}

		public override bool Predicate(Ship ship)
		{
			if (this.Both) return true;
			if (this.Fast && ship.Info.Speed == ShipSpeed.Fast) return true;
			if (this.Low && ship.Info.Speed == ShipSpeed.Low) return true;

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
				this.Name = area?.Name ?? "出撃海域なし";
			}

			public bool Predicate(Ship ship)
			{
				return this.IsChecked && this.model.Area == ship.SallyArea;
			}
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
}
