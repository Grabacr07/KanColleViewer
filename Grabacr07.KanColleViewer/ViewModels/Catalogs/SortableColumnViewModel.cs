using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleWrapper.Models;
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

		public abstract IEnumerable<Ship> Sort(IEnumerable<Ship> list);
	}

	public class NoneColumnViewModel : SortableColumnViewModel
	{
		public NoneColumnViewModel() : base(ShipCatalogSortTarget.None) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			return list;
		}
	}

	public class IdColumnViewModel : SortableColumnViewModel
	{
		public IdColumnViewModel() : base(ShipCatalogSortTarget.Id) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class TypeColumnViewModel : SortableColumnViewModel
	{
		public TypeColumnViewModel() : base(ShipCatalogSortTarget.Type) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Info.SortId)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Info.ShipType.Id)
					.ThenByDescending(x => x.Info.SortId)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class NameColumnViewModel : SortableColumnViewModel
	{
		public NameColumnViewModel() : base(ShipCatalogSortTarget.Name) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderBy(x => x.Info.SortId)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderByDescending(x => x.Info.SortId)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class LevelColumnViewModel : SortableColumnViewModel
	{
		public LevelColumnViewModel() : base(ShipCatalogSortTarget.Level) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderByDescending(x => x.Level)
					.ThenBy(x => x.Info.SortId)
					.ThenBy(x => x.Id);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderBy(x => x.Level)
					.ThenByDescending(x => x.Info.SortId)
					.ThenByDescending(x => x.Id);
			}
			return list;
		}
	}

	public class ConditionColumnViewModel : SortableColumnViewModel
	{
		public ConditionColumnViewModel() : base(ShipCatalogSortTarget.Cond) { }

		public override IEnumerable<Ship> Sort(IEnumerable<Ship> list)
		{
			if (this.Direction == SortDirection.Ascending)
			{
				return list.OrderByDescending(x => x.Condition)
					.ThenBy(x => x.Info.ShipType.Id)
					.ThenBy(x => x.Level)
					.ThenBy(x => x.Info.SortId);
			}
			if (this.Direction == SortDirection.Descending)
			{
				return list.OrderBy(x => x.Condition)
					.ThenByDescending(x => x.Info.ShipType.Id)
					.ThenByDescending(x => x.Level)
					.ThenByDescending(x => x.Info.SortId);
			}
			return list;
		}
	}
}
