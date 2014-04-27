using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			if (this.action != null) this.action();
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
			if (this.Fast && ship.Info.Speed == Speed.Fast) return true;
			if (this.Low && ship.Info.Speed == Speed.Low) return true;

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
}
