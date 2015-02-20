using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class FleetWindowViewModel : WindowViewModel
	{
		private FleetViewModel[] allFleets;

		public Organization Organization
		{
			get { return KanColleClient.Current.Homeport.Organization; }
		}

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
			this.Title = "艦隊詳細";
			this.Fleets = new ItemViewModel[0];

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Organization)
			{
				// Fleets の PropertyChanged が来るのは、最初と艦隊数が増えたときくらい っぽい
				{ "Fleets", (sender, args) => this.InitializeFleets() },
				{ "Combined", (sender, args) => this.UpdateFleets() },
				{ "CombinedFleet", (sender, args) => this.UpdateFleets() },
			});

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
