using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class FleetWindowViewModel : WindowViewModel
	{
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

		#region SelectedFleet 変更通知プロパティ

		private FleetViewModel _SelectedFleet;

		/// <summary>
		/// 現在選択されている艦隊を取得または設定します。
		/// </summary>
		public FleetViewModel SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (this._SelectedFleet != value)
				{
					if (this._SelectedFleet != null) this.SelectedFleet.IsSelected = false;
					if (value != null) value.IsSelected = true;
					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public FleetWindowViewModel()
		{
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Organization)
			{
				{ "Fleets", (sender, args) => this.UpdateFleets() },
			});
			this.UpdateFleets();
		}

		private void UpdateFleets()
		{
			this.Fleets = KanColleClient.Current.Homeport.Organization.Fleets.Select(kvp => new FleetViewModel(kvp.Value)).ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}
	}

	public class CombinedFleetViewModel : ViewModel
	{
		private readonly Fleet source1;
		private readonly Fleet source2;

		public bool IsCombined { get; private set; }

		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ShipViewModel[] Ships1
		{
			get { return this.source1.Ships.Select(x => new ShipViewModel(x)).ToArray(); }
		}

		public ShipViewModel[] Ships2
		{
			get { return this.source2.Ships.Select(x => new ShipViewModel(x)).ToArray(); }
		}

		public CombinedFleetViewModel(Fleet source1, Fleet source2)
		{
			this.source1 = source1;
			this.source2 = source2;
			this.IsCombined = true;
		}
	}


	public abstract class FleetViewModelBase : ViewModel
	{
		public abstract ViewModel State { get; }

		public abstract string Name { get; }

		public abstract string TotalLevel { get; }

		public abstract string AverageLevel { get; }

		public abstract string Speed { get; }

		public abstract int AirSuperiorityPotential { get; }

		public abstract string TotalViewRange { get; }


		public void RaiseWrapperPropertyChanged()
		{
			// 個別に用意するの面倒なのでまとめちゃえ系実装

			this.RaisePropertyChanged("Name");
			this.RaisePropertyChanged("TotalLevel");
			this.RaisePropertyChanged("AverageLevel");
			this.RaisePropertyChanged("Speed");
			this.RaisePropertyChanged("AirSuperiorityPotential");
			this.RaisePropertyChanged("ViewRange");
		}
	}
}
