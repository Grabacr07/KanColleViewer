﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class FleetWindowViewModel : WindowViewModel
	{
		private FleetViewModel[] allFleets;

		#region Fleets 変更通知プロパティ

		private ItemViewModel[] _Fleets;

		public ItemViewModel[] Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SelectedFleet 変更通知プロパティ

		private ItemViewModel _SelectedFleet;

		/// <summary>
		/// 現在選択されている艦隊を取得または設定します。
		/// </summary>
		public ItemViewModel SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (this._SelectedFleet != value)
				{
					if (this._SelectedFleet != null) this._SelectedFleet.IsSelected = false;
					if (value != null) value.IsSelected = true;

					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public FleetWindowViewModel()
		{
			this.Title = "함대 상세정보";
			this.Fleets = new ItemViewModel[0];

			KanColleClient.Current.Homeport.Organization
				.Subscribe(nameof(Organization.Fleets), this.InitializeFleets)
				.Subscribe(nameof(Organization.Combined), this.UpdateFleets)
				.Subscribe(nameof(Organization.CombinedFleet), this.UpdateFleets)
				.AddTo(this);

			this.InitializeFleets();
		}


		private void InitializeFleets()
		{
			this.allFleets = KanColleClient.Current.Homeport.Organization.Fleets.Select(kvp => new FleetViewModel(kvp.Value)).ToArray();
			this.UpdateFleets();
		}

		private void UpdateFleets()
		{
			// ややこしいけど、CombinedFleetViewModel は連合艦隊が編成・解除される度に使い捨て
			// FleetViewModel は InitializeFleets() で作ったインスタンスをずっと使う

			foreach (var f in this.Fleets.OfType<CombinedFleetViewModel>()) f.Dispose();

			if (KanColleClient.Current.Homeport.Organization.Combined)
			{
				var cfvm = new CombinedFleetViewModel(KanColleClient.Current.Homeport.Organization.CombinedFleet);
				var fleets = this.allFleets.Where(x => cfvm.Source.Fleets.All(f => f != x.Source));

				this.Fleets = EnumerableEx.Return<ItemViewModel>(cfvm).Concat(fleets).ToArray();
				this.SelectedFleet = cfvm;
			}
			else
			{
				this.Fleets = this.allFleets.OfType<ItemViewModel>().ToArray();

				if (this.allFleets.All(x => x != this.SelectedFleet))
				{
					// SelectedFleet が allFleets の中のどれでもないとき
					// -> SelectedFleet は連合艦隊だったので、改めて第一艦隊を選択
					this.SelectedFleet = this.Fleets.FirstOrDefault();
				}
			}
		}
	}
}
