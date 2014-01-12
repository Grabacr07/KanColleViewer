using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public abstract class SortableColumnViewModel : ViewModel
	{
		public ShipCatalogSortTarget Target { get; private set; }

		#region Direction 変更通知プロパティ

		private SortDirection _Direction = SortDirection.None;

		public SortDirection Direction
		{
			get { return this._Direction; }
			set
			{
				if (this._Direction != value)
				{
					this._Direction = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		protected SortableColumnViewModel(ShipCatalogSortTarget target)
		{
			this.Target = target;
		}

		public abstract IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list);
	}

	public class NoneColumnViewModel : SortableColumnViewModel
	{
		public NoneColumnViewModel() : base(ShipCatalogSortTarget.None) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			return list;
		}
	}

	public class IdColumnViewModel : SortableColumnViewModel
	{
		public IdColumnViewModel() : base(ShipCatalogSortTarget.Id) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Ship.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Ship.Id);
			}
			return list;
		}
	}

	public class TypeColumnViewModel : SortableColumnViewModel
	{
		public TypeColumnViewModel() : base(ShipCatalogSortTarget.Type) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Ship.Info.ShipType.Id)
					.ThenBy(x => x.Ship.Info.SortId)
					.ThenBy(x => x.Ship.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Ship.Info.ShipType.Id)
					.ThenByDescending(x => x.Ship.Info.SortId)
					.ThenByDescending(x => x.Ship.Id);
			}
			return list;
		}
	}

	public class NameColumnViewModel : SortableColumnViewModel
	{
		public NameColumnViewModel() : base(ShipCatalogSortTarget.Name) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Ship.Info.SortId)
					.ThenBy(x => x.Ship.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Ship.Info.SortId)
					.ThenByDescending(x => x.Ship.Id);
			}
			return list;
		}
	}

	public class LevelColumnViewModel : SortableColumnViewModel
	{
		public LevelColumnViewModel() : base(ShipCatalogSortTarget.Level) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderByDescending(x => x.Ship.Level)
					.ThenBy(x => x.Ship.Info.SortId)
					.ThenBy(x => x.Ship.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderBy(x => x.Ship.Level)
					.ThenByDescending(x => x.Ship.Info.SortId)
					.ThenByDescending(x => x.Ship.Id);
			}
			return list;
		}
	}

	public class ConditionColumnViewModel : SortableColumnViewModel
	{
		public ConditionColumnViewModel() : base(ShipCatalogSortTarget.Cond) { }

		public override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderByDescending(x => x.Ship.Condition)
					.ThenBy(x => x.Ship.Info.ShipType.Id)
					.ThenBy(x => x.Ship.Level)
					.ThenBy(x => x.Ship.Info.SortId);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderBy(x => x.Ship.Condition)
					.ThenByDescending(x => x.Ship.Info.ShipType.Id)
					.ThenByDescending(x => x.Ship.Level)
					.ThenByDescending(x => x.Ship.Info.SortId);
			}
			return list;
		}
	}
}
