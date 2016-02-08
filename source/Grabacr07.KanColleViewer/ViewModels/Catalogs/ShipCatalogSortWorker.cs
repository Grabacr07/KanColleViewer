using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogSortWorker : ViewModel
	{
		#region static members

		private const int selectorNum = 4;
		public static readonly SortableColumn NoneColumn = new SortableColumn { Name = Resources.ShipCatalog_Sort_None, KeySelector = null };
		public static readonly SortableColumn IdColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_ObtainedID, KeySelector = x => x.Id, };
		public static readonly SortableColumn StypeColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_HullCode, KeySelector = x => x.Info.ShipType.SortNumber, };
		public static readonly SortableColumn SortIdColumn = new SortableColumn { Name = Resources.ShipCatalog_SortBy_CatalogID, KeySelector = x => x.Info.SortId, };
		public static readonly SortableColumn NameColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Name, KeySelector = x => x.Info.Name, };
		public static readonly SortableColumn LevelColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Level, KeySelector = x => x.Level, DefaultIsDescending = true, };
		public static readonly SortableColumn ExpColumn = new SortableColumn { Name = Resources.ShipCatalog_SortBy_Experience, KeySelector = x => x.ExpForNextLevel, };
		public static readonly SortableColumn CondColumn = new SortableColumn { Name = Resources.ShipCatalog_SortBy_Condition, KeySelector = x => x.Condition, DefaultIsDescending = true, };
		public static readonly SortableColumn FirepowerColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Firepower, KeySelector = x => x.Firepower.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn TorpedoColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Torpedo, KeySelector = x => x.Torpedo.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn AAColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_AntiAir, KeySelector = x => x.AA.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn ArmerColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Armour, KeySelector = x => x.Armer.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn LuckColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Luck, KeySelector = x => x.Luck.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn HPColumn = new SortableColumn { Name = Resources.ShipCatalog_SortBy_MaxHP, KeySelector = x => x.HP.Maximum, DefaultIsDescending = true, };
		public static readonly SortableColumn ViewRangeColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_ViewRange, KeySelector = x => x.ViewRange, DefaultIsDescending = true, };
		public static readonly SortableColumn EvasionColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_Evasion, KeySelector = x => x.Evasion.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn AntiSubColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_ASW, KeySelector = x => x.AntiSub.Current, DefaultIsDescending = true, };
		public static readonly SortableColumn TimeToRepairColumn = new SortableColumn { Name = Resources.ShipCatalog_Column_TTR, KeySelector = x => x.TimeToRepair.Ticks, DefaultIsDescending = true, };

		public static SortableColumn[] Columns { get; set; }

		static ShipCatalogSortWorker()
		{
			Columns = new[]
			{
				NoneColumn,
				IdColumn,
				StypeColumn,
				SortIdColumn,
				NameColumn,
				LevelColumn,
				CondColumn,
				FirepowerColumn,
				TorpedoColumn,
				AAColumn,
				ArmerColumn,
				LuckColumn,
				HPColumn,
				ViewRangeColumn,
				EvasionColumn,
				AntiSubColumn,
				TimeToRepairColumn,
			};
		}

		#endregion

		#region Selectors 変更通知プロパティ

		private SortableColumnSelector[] _Selectors;

		public SortableColumnSelector[] Selectors
		{
			get { return this._Selectors; }
			set
			{
				if (this._Selectors != value)
				{
					this._Selectors = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public ShipCatalogSortWorker()
		{
			this.UpdateSelectors();

			this.SetFirst(LevelColumn);
		}

		public IEnumerable<Ship> Sort(IEnumerable<Ship> ships)
		{
			var selectors = this.Selectors.Where(x => x.Current.KeySelector != null).ToArray();
			if (selectors.Length == 0) return ships;

			var selector = selectors[0].Current.KeySelector;
			var orderedShips = selectors[0].IsAscending ? ships.OrderBy(selector) : ships.OrderByDescending(selector);

			for (var i = 1; i < selectors.Length; i++)
			{
				selector = selectors[i].Current.KeySelector;
				orderedShips = selectors[i].IsAscending ? orderedShips.ThenBy(selector) : orderedShips.ThenByDescending(selector);
			}

			return orderedShips;
		}

		public void SetFirst(SortableColumn column)
		{
			if (!this.Selectors.HasItems()) return;

			if (column == StypeColumn)
			{
				// 列ヘッダーから艦種が選択されたときは、艦種 (降順) -> 艦名 (昇順) に設定
				// (ゲーム内の艦種ソートと同じ動作)

				this.Selectors[0].SafeUpdate(StypeColumn);
				this.Selectors[0].SafeUpdate(false);
				if (this.Selectors.Length >= 2)
				{
					this.Selectors[1].SafeUpdate(NameColumn);
					this.Selectors[1].SafeUpdate(true);
				}
			}
			else
			{
				this.Selectors[0].SafeUpdate(column);
				this.Selectors[0].SafeUpdate(!column.DefaultIsDescending);
			}

			this.UpdateSelectors();
		}

		public void Clear()
		{
			this.Selectors = null;
			this.UpdateSelectors();
		}

		private void UpdateSelectors(SortableColumnSelector target = null)
		{
			if (this.Selectors == null)
			{
				this.Selectors = Enumerable.Range(0, selectorNum)
					.Select(_ => new SortableColumnSelector { Updated = x => this.UpdateSelectors(x), })
					.ToArray();
			}

			// nonColumn 以外で選択された列
			var selectedItems = new HashSet<SortableColumn>();
			SortableColumnSelector previous = null;

			// enabled は Selector の SelectableColumns を作り直すかどうか
			// target が指定されていなければ全部、指定されていればその target の次から作り直す
			var enabled = target == null;

			foreach (var selector in this.Selectors)
			{
				if (enabled)
				{
					var sortables = Columns.Where(x => !selectedItems.Contains(x)).ToList();
					var current = selector.Current;

					if (previous != null && previous.Current == LevelColumn)
					{
						// 直前のソート列がレベルだったら、この列は次のレベルまでの経験値にしてあげる
						sortables.Insert(1, ExpColumn);
						current = ExpColumn;
						selector.SafeUpdate(!previous.IsAscending);
					}

					selector.SelectableColumns = sortables.ToArray();
					selector.SafeUpdate(sortables.Contains(current) ? current : sortables.FirstOrDefault());
				}
				else
				{
					enabled = selector == target;
				}

				if (selector.Current != NoneColumn)
				{
					selectedItems.Add(selector.Current);
				}

				previous = selector;
			}
		}
	}


	public class SortableColumnSelector : ViewModel
	{
		internal Action<SortableColumnSelector> Updated { get; set; }

		#region Current 変更通知プロパティ

		private SortableColumn _Current;

		public SortableColumn Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.SafeUpdate(!value.DefaultIsDescending);
					this.Updated(this);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SelectableColumns 変更通知プロパティ

		private SortableColumn[] _SelectableColumns;

		public SortableColumn[] SelectableColumns
		{
			get { return this._SelectableColumns; }
			set
			{
				if (this._SelectableColumns != value)
				{
					this._SelectableColumns = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsAscending / IsDescending 変更通知プロパティ

		private bool _IsAscending = true;

		public bool IsAscending
		{
			get { return this._IsAscending; }
			set
			{
				if (this._IsAscending != value)
				{
					this._IsAscending = value;
					this.Updated(this);
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.IsDescending));
				}
			}
		}

		public bool IsDescending => !this.IsAscending;

		#endregion

		internal void SafeUpdate(SortableColumn column)
		{
			this._Current = column;
			this.RaisePropertyChanged(nameof(this.Current));
		}

		internal void SafeUpdate(bool isAscending)
		{
			this._IsAscending = isAscending;
			this.RaisePropertyChanged(nameof(this.IsAscending));
			this.RaisePropertyChanged(nameof(this.IsDescending));
		}
	}

	public class SortableColumn
	{
		public string Name { get; set; }
		public bool DefaultIsDescending { get; set; }
		public Func<Ship, object> KeySelector { get; set; }
	}
}
