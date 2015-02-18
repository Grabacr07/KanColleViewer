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

		private FleetViewModel[] _Fleets;

		public FleetViewModel[] Fleets
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

		#region CombinedFleets 変更通知プロパティ

		private CombinedFleetViewModel[] _CombinedFleets;

		public CombinedFleetViewModel[] CombinedFleets
		{
			get { return this._CombinedFleets; }
			set
			{
				if (this._CombinedFleets != value)
				{
					this._CombinedFleets = value;
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
					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public FleetWindowViewModel()
		{
			this.Title = "艦隊詳細";

			this.Fleets = new FleetViewModel[0];
			this.CombinedFleets = new CombinedFleetViewModel[0];

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
			foreach (var f in this.CombinedFleets.Where(x => x != null)) f.Dispose();

			if (KanColleClient.Current.Homeport.Organization.Combined)
			{
				var cfvm = new CombinedFleetViewModel(KanColleClient.Current.Homeport.Organization.CombinedFleet);

				this.CombinedFleets = new[] { cfvm, };
				this.Fleets = this.allFleets
					.Where(x => this.CombinedFleets.SelectMany(cf => cf.Source.Fleets).All(f => f != x.Source))
					.ToArray();
				this.SelectedFleet = cfvm;
			}
			else
			{
				this.CombinedFleets = new CombinedFleetViewModel[0];
				this.Fleets = this.allFleets;

				if (this.allFleets.All(x => x != this.SelectedFleet))
				{
					// SelectedFleet が allFleets の中のどれでもないとき
					// -> SelectedFleet は連合艦隊だったので、改めて第一艦隊を選択
					this.SelectedFleet = this.allFleets.FirstOrDefault();
				}
			}
		}
	}
}
