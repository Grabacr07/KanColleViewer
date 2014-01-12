using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.Views.Catalogs;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipCatalogWindowViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		public ShipCatalogSortWorker SortWorker { get; private set; }
		public IReadOnlyCollection<ShipTypeViewModel> ShipTypes { get; private set; }

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

		#region IsOpenSettings 変更通知プロパティ

		private bool _IsOpenSettings;

		public bool IsOpenSettings
		{
			get { return this._IsOpenSettings; }
			set
			{
				if (this._IsOpenSettings != value)
				{
					this._IsOpenSettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region WithoutLv1Ship 変更通知プロパティ

		private bool _WithoutLv1Ship = true;

		/// <summary>
		/// Lv.1 の艦を除くかどうかを示す値を取得または設定します。
		/// </summary>
		public bool WithoutLv1Ship
		{
			get { return this._WithoutLv1Ship; }
			set
			{
				if (this._WithoutLv1Ship != value)
				{
					this._WithoutLv1Ship = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

		#endregion

		#region WithoutMaxModernizedShip 変更通知プロパティ

		private bool _WithoutMaxModernizedShip;

		/// <summary>
		/// 全ステータスの近代化改修が完了している艦を除くかどうかを示す値を取得または設定します。
		/// </summary>
		public bool WithoutMaxModernizedShip
		{
			get { return this._WithoutMaxModernizedShip; }
			set
			{
				if (this._WithoutMaxModernizedShip != value)
				{
					this._WithoutMaxModernizedShip = value;
					this.RaisePropertyChanged();
					this.Update();
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
			this.IsOpenSettings = true;

			this.SortWorker = new ShipCatalogSortWorker();

			this.ShipTypes = KanColleClient.Current.Master.ShipTypes
				.Select(kvp => new ShipTypeViewModel(kvp.Value)
				{
					IsSelected = true,
					SelectionChangedAction = () => this.Update()
				})
				.ToList();

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.Throttle(TimeSpan.FromMilliseconds(500.0))
				.Do(_ => this.UpdateCore())
				.Subscribe(_ => this.IsReloading = false);
			this.CompositeDisposable.Add(this.updateSource);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport)
			{
				{ () => this.homeport.Ships, (sender, args) => this.Update() },
			});

			this.Update();
		}


		public void Update()
		{
			this.RaisePropertyChanged("AllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}

		public void Update(ShipCatalogSortTarget sortTarget)
		{
			this.SortWorker.SetTarget(sortTarget);
			this.Update();
		}

		private void UpdateCore()
		{
			var list = this.homeport.Ships
				.Where(x => this.ShipTypes.Where(t => t.IsSelected).Any(t => x.Value.Info.ShipType.Id == t.Id))
				.Where(x => !this.WithoutLv1Ship || x.Value.Level != 1)
				.Where(x => !this.WithoutMaxModernizedShip || !x.Value.IsMaxModernized)
				.Select(x => new ShipViewModel(x.Value));

			this.Ships = this.SortWorker.Sort(list).ToList();
		}
	}
}
