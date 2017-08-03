﻿using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using MetroTrilithon.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class NeedNdockShipCatalogWindowViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		public ShipCatalogSortWorker SortWorker { get; private set; }
		public ShipDamagedFilter ShipDamagedFilter { get; }

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


		public NeedNdockShipCatalogWindowViewModel()
		{
			this.SortWorker = new ShipCatalogSortWorker(false);

			this.Title = "입거 필요 칸무스 목록";
			this.ShipDamagedFilter = new ShipDamagedFilter(this.Update);

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.SelectMany(x => this.UpdateAsync())
				.Do(_ => this.IsReloading = false)
				.Subscribe()
				.AddTo(this);

			this.CompositeDisposable.Add(this.updateSource);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Organization)
			{
				{ "Ships", (sender, args) => this.Update() },
			});

			this.Update();
		}
		public void Update()
		{
			this.RaisePropertyChanged("CheckAllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}
		private IObservable<Unit> UpdateAsync()
		{
			return Observable.Start(() =>
			{
				var list = this.homeport.Organization.Ships
					.Select(kvp => kvp.Value)
					.Where(this.ShipDamagedFilter.Predicate)
					.Where(x => x.TimeToRepair != TimeSpan.Zero)
					.Where(x => !x.Situation.HasFlag(KanColleWrapper.Models.ShipSituation.Repair));

				this.Ships = this.SortWorker.Sort(list)
					.Select((x, i) => new ShipViewModel(i + 1, x, null))
					.ToList();
			});
		}
		public void Sort(SortableColumn column)
		{
			this.SortWorker.SetFirst(column);
			this.Update();
		}
	}
}
