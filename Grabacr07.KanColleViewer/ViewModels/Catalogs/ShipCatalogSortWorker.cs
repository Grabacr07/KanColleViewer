using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogSortWorker
	{
		private readonly List<SortableColumnViewModel> sortableColumns;
		private readonly NoneColumnViewModel noneColumn = new NoneColumnViewModel();
		private SortableColumnViewModel currentSortTarget;

		public IdColumnViewModel IdColumn { get; private set; }
		public TypeColumnViewModel TypeColumn { get; private set; }
		public NameColumnViewModel NameColumn { get; private set; }
		public LevelColumnViewModel LevelColumn { get; private set; }
		public ConditionColumnViewModel ConditionColumn { get; private set; }
		public ViewRangeColumnViewModel ViewRangeColumn { get; private set; }

		public ShipCatalogSortWorker()
		{
			this.IdColumn = new IdColumnViewModel();
			this.TypeColumn = new TypeColumnViewModel();
			this.NameColumn = new NameColumnViewModel();
			this.LevelColumn = new LevelColumnViewModel();
			this.ConditionColumn = new ConditionColumnViewModel();
			this.ViewRangeColumn = new ViewRangeColumnViewModel();

			this.sortableColumns = new List<SortableColumnViewModel>
			{
				this.noneColumn,
				this.IdColumn,
				this.TypeColumn,
				this.NameColumn,
				this.LevelColumn,
				this.ConditionColumn,
				this.ViewRangeColumn,
			};

			this.currentSortTarget = this.noneColumn;
		}

		public void SetTarget(ShipCatalogSortTarget sortTarget, bool reverse)
		{
			var target = this.sortableColumns.FirstOrDefault(x => x.Target == sortTarget);
			if (target == null) return;

			if (reverse)
			{
				switch (target.Direction)
				{
					case SortDirection.None:
						target.Direction = SortDirection.Descending;
						break;
					case SortDirection.Descending:
						target.Direction = SortDirection.Ascending;
						break;
					case SortDirection.Ascending:
						target = this.noneColumn;
						break;
				}
			}
			else
			{
				switch (target.Direction)
				{
					case SortDirection.None:
						target.Direction = SortDirection.Ascending;
						break;
					case SortDirection.Ascending:
						target.Direction = SortDirection.Descending;
						break;
					case SortDirection.Descending:
						target = this.noneColumn;
						break;
				}
			}

			this.currentSortTarget = target;
			this.sortableColumns.Where(x => x.Target != target.Target).ForEach(x => x.Direction = SortDirection.None);
		}

		public IEnumerable<Ship> Sort(IEnumerable<Ship> shipList)
		{
			return this.currentSortTarget.Sort(shipList);
		}
	}
}
