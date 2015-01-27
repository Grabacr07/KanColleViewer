using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class NeedNdockShipCatalogWindowViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;
		public ShipCatalogSortWorker SortWorker { get; private set; }
		public ShipNdockTimeFilter ShipNdockTimeFilter { get; private set; }
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
			this.SortWorker = new ShipCatalogSortWorker();
			this.SortWorker.SetTarget(ShipCatalogSortTarget.RepairTime, true);

			this.Title = "입거 필요 칸무스 목록";
			this.ShipNdockTimeFilter = new ShipNdockTimeFilter(this.Update);

			this.updateSource
				.Do(_ => this.IsReloading = true)
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
			this.RaisePropertyChanged("CheckAllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}
		public void Update(ShipCatalogSortTarget sortTarget)
		{
			this.SortWorker.SetTarget(sortTarget, false);
			this.Update();
		}
		public void UpdateReverse(ShipCatalogSortTarget sortTarget)
		{
			this.SortWorker.SetTarget(sortTarget, true);
			this.Update();
		}
		private void UpdateCore()
		{
			var list = this.homeport.Organization.Ships
				.Select(kvp => kvp.Value)
				.Where(this.ShipNdockTimeFilter.Predicate);

			this.Ships = this.SortWorker.Sort(list)
				.Select((x, i) => new ShipViewModel(i + 1, x))
				.ToList();
		}
	}
}
