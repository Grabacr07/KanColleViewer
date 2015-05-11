﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogWindowViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		public ShipCatalogSortWorker SortWorker { get; private set; }
		public IReadOnlyCollection<ShipTypeViewModel> ShipTypes { get; private set; }

		public ShipLevelFilter ShipLevelFilter { get; private set; }
		public ShipLockFilter ShipLockFilter { get; private set; }
		public ShipSpeedFilter ShipSpeedFilter { get; private set; }
		public ShipModernizeFilter ShipModernizeFilter { get; private set; }
		public ShipRemodelingFilter ShipRemodelingFilter { get; private set; }
		public ShipExpeditionFilter ShipExpeditionFilter { get; private set; }
		public ShipSallyAreaFilter ShipSallyAreaFilter { get; private set; }

		public bool CheckAllShipTypes
		{
			get { return this.ShipTypes.All(x => x.IsSelected); }
			set
			{
				this.ShipTypes.ForEach(x => x.Set(value));
				this.Update();
			}
		}

		#region Ships 変更通知プロパティ

		private IReadOnlyCollection<ShipViewModel> _Ships;

		public IReadOnlyCollection<ShipViewModel> Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsOpenFilterSettings 変更通知プロパティ

		private bool _IsOpenFilterSettings;

		public bool IsOpenFilterSettings
		{
			get { return this._IsOpenFilterSettings; }
			set
			{
				if (this._IsOpenFilterSettings != value)
				{
					this._IsOpenFilterSettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsOpenSortSettings 変更通知プロパティ

		private bool _IsOpenSortSettings;

		public bool IsOpenSortSettings
		{
			get { return this._IsOpenSortSettings; }
			set
			{
				if (this._IsOpenSortSettings != value)
				{
					this._IsOpenSortSettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsReloading 変更通知プロパティ

		private bool _IsReloading;

		public bool IsReloading
		{
			get { return this._IsReloading; }
			set
			{
				if (this._IsReloading != value)
				{
					this._IsReloading = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public ShipCatalogWindowViewModel()
		{
			this.Title = "所属艦娘一覧";
			this.IsOpenFilterSettings = true;

			this.SortWorker = new ShipCatalogSortWorker();

			this.ShipTypes = KanColleClient.Current.Master.ShipTypes
				.Select(kvp => new ShipTypeViewModel(kvp.Value)
				{
					IsSelected = true,
					SelectionChangedAction = () => this.Update()
				})
				.ToList();

			this.ShipLevelFilter = new ShipLevelFilter(this.Update);
			this.ShipLockFilter = new ShipLockFilter(this.Update);
			this.ShipSpeedFilter = new ShipSpeedFilter(this.Update);
			this.ShipModernizeFilter = new ShipModernizeFilter(this.Update);
			this.ShipRemodelingFilter = new ShipRemodelingFilter(this.Update);
			this.ShipExpeditionFilter = new ShipExpeditionFilter(this.Update);
			this.ShipSallyAreaFilter = new ShipSallyAreaFilter(this.Update);

			this.updateSource
				.Do(_ => this.IsReloading = true)
				// ☟ 連続で艦種選択できるように猶予を設けるつもりだったけど、
				// 　 ソートだけしたいケースとかだと遅くてイラ壁なので迷う
				.Throttle(TimeSpan.FromMilliseconds(7.0))
				.Do(_ => this.UpdateCore())
				.Subscribe(_ => this.IsReloading = false);
			this.CompositeDisposable.Add(this.updateSource);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Organization)
			{
				{ "Ships", (sender, args) => this.Update() },
			});

			this.Update();
		}


		public void Update()
		{
			this.ShipExpeditionFilter.SetFleets(this.homeport.Organization.Fleets);

			this.RaisePropertyChanged("CheckAllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}

		private void UpdateCore()
		{
			var list = this.homeport.Organization.Ships
				.Select(kvp => kvp.Value)
				.Where(x => this.ShipTypes.Where(t => t.IsSelected).Any(t => x.Info.ShipType.Id == t.Id))
				.Where(this.ShipLevelFilter.Predicate)
				.Where(this.ShipLockFilter.Predicate)
				.Where(this.ShipSpeedFilter.Predicate)
				.Where(this.ShipModernizeFilter.Predicate)
				.Where(this.ShipRemodelingFilter.Predicate)
				.Where(this.ShipExpeditionFilter.Predicate)
				.Where(this.ShipSallyAreaFilter.Predicate);

			this.Ships = this.SortWorker.Sort(list)
				.Select((x, i) => new ShipViewModel(i + 1, x))
				.ToList();
		}


		public void SetShipType(int[] ids)
		{
			this.ShipTypes.ForEach(x => x.Set(ids.Any(id => x.Id == id)));
			this.Update();
		}

		public void Sort(SortableColumn column)
		{
			this.SortWorker.SetFirst(column);
			this.Update();
		}
	}
}
