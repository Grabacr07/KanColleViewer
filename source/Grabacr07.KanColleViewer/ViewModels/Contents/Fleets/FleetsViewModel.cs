﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.Messaging;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using StatefulModel;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleViewer.Views.Catalogs;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class FleetsViewModel : TabItemViewModel
	{
		private MultipleDisposable fleetListeners;

		public override string Name
		{
			get { return Properties.Resources.Fleets; }
			protected set { throw new NotImplementedException(); }
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
		private ItemViewModel _SelectedFleet;

		/// <summary>
		/// 現在選択されている艦隊を取得または設定します。
		/// </summary>
		public ItemViewModel SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (_SelectedFleet == value) return; // 같아서 문제?

				if (this.SelectedFleet != null)
					this.SelectedFleet.IsSelected = false;

				if (value != null)
					value.IsSelected = true;

				this._SelectedFleet = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Fleets2 변경통지 프로퍼티 (연합함대를 포함하는 리스트)
		private ItemViewModel[] _Fleets2;
		public ItemViewModel[] Fleets2
		{
			get { return this._Fleets2; }
			set
			{
				if (this._Fleets2 != value)
				{
					this._Fleets2 = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AirBase 変更通知プロパティ
		private AirBaseTroopViewModel _AirBase;
		public AirBaseTroopViewModel AirBase
		{
			get { return this._AirBase; }
			set
			{
				if (this._AirBase != value)
				{
					this._AirBase = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ShowLostAirplane 변경통지 프로퍼티

		private bool _ShowLostAirplane;

		public bool ShowLostAirplane
		{
			get { return this._ShowLostAirplane; }
			set
			{
				if (this._ShowLostAirplane != value)
				{
					this._ShowLostAirplane = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShowAirplaneAlways 변경통지 프로퍼티

		private bool _ShowAirplaneAlways;

		public bool ShowAirplaneAlways
		{
			get { return this._ShowAirplaneAlways; }
			set
			{
				if (this._ShowAirplaneAlways != value)
				{
					this._ShowAirplaneAlways = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private bool prevCombined { get; set; }

		public FleetsViewModel()
		{
			this.Fleets2 = new ItemViewModel[0];

			KanColleClient.Current.Homeport.Organization
				.Subscribe(nameof(Organization.Fleets), this.UpdateFleets)
				.Subscribe(nameof(Organization.Combined), this.UpdateFleets)
				.Subscribe(nameof(Organization.CombinedFleet), this.UpdateFleets)
				.AddTo(this);

			AirBase = new AirBaseTroopViewModel(KanColleClient.Current.Proxy);

			Disposable
				.Create(() => this.fleetListeners?.Dispose())
				.AddTo(this);

			KanColleSettings.ShowLostAirplane.ValueChanged += (s, e) => this.ShowLostAirplane = e.NewValue;
			KanColleSettings.ShowAirplaneAlways.ValueChanged += (s, e) => this.ShowAirplaneAlways = e.NewValue;
			KanColleSettings.MergeCombinedFleet.ValueChanged += (s, e) => this.UpdateFleets();
			this.ShowLostAirplane = KanColleSettings.ShowLostAirplane;
			this.ShowAirplaneAlways = KanColleSettings.ShowAirplaneAlways;

			prevCombined = KanColleClient.Current.Homeport.Organization.Combined;
		}

		public void ShowFleetWindow()
		{
			var fleetwd = new FleetWindowViewModel();
			var message = new TransitionMessage(fleetwd, TransitionMode.Normal, "FleetWindow.Show");
			this.Messenger.Raise(message);
		}


		private async void UpdateFleets()
		{
			this.fleetListeners?.Dispose();
			this.fleetListeners = new MultipleDisposable();

			this.Fleets = KanColleClient.Current.Homeport.Organization.Fleets
				.Select(x => this.ToViewModel(x.Value))
				.ToArray();

			IEnumerable<ItemViewModel> result;

			if (KanColleClient.Current.Homeport.Organization.Combined && KanColleSettings.MergeCombinedFleet)
			{
				var cfvm = MakeCombinedFleetViewModel(KanColleClient.Current.Homeport.Organization.CombinedFleet);
				var fleets = this.Fleets.Where(x => cfvm.Source.Fleets.All(f => f != x.Source));

				result = EnumerableEx.Return<ItemViewModel>(cfvm)
					.Concat(fleets.Select(x => new FleetViewModel(x.Source)));
			}
			else
			{
				result = this.Fleets
					.Select(x => new FleetViewModel(x.Source))
					.OfType<ItemViewModel>();
			}

			{
				var list = result.ToList();
				list.Add(this.AirBase);
				result = list;
			}
			this.Fleets2 = result.ToArray();

			// SelectedFleet 이 무시되는 현상. 이유는 불명.
			await Task.Delay(200);

			if (!this.Fleets2.Any(x => x == this.SelectedFleet))
				this.SelectedFleet = this.Fleets2.FirstOrDefault();

			// 연합 함대 해제 혹은 설정인 경우 메모리 정리...
			if (KanColleClient.Current.Homeport.Organization.Combined || prevCombined)
				GCWorker.GCRequest(-1, GCWorker.GCType.GCAll);

			prevCombined = KanColleClient.Current.Homeport.Organization.Combined;
		}

		private CombinedFleetViewModel combinedFleetInstance;
		private CombinedFleetViewModel MakeCombinedFleetViewModel(CombinedFleet fleet)
		{
			if (combinedFleetInstance == null || combinedFleetInstance.Source != fleet)
				combinedFleetInstance = new CombinedFleetViewModel(fleet);

			return combinedFleetInstance;
		}

		private FleetViewModel ToViewModel(Fleet fleet)
		{
			var vm = new FleetViewModel(fleet).AddTo(this.fleetListeners);
			fleet.Subscribe(nameof(Fleet.ShipsUpdated), () =>
			{
				if (KanColleSettings.AutoFleetSelectWhenShipsChanged)
				{
					var first = this.Fleets2.FirstOrDefault();

					// 첫번째가 연합함대
					if (first.GetType() == typeof(CombinedFleetViewModel))
					{
						// 연합함대에 포함
						if ((first as CombinedFleetViewModel).Source.Fleets.Any(x => x == fleet))
							this.SelectedFleet = first;
						else
							this.SelectedFleet = vm;
					}
					else this.SelectedFleet = vm;
				}
			}, false).AddTo(this.fleetListeners);

			fleet.Subscribe(nameof(Fleet.IsInSortie), () => {
				if (KanColleSettings.AutoFleetSelectWhenSortie)
				{
					var first = this.Fleets2.FirstOrDefault();

					// 첫번째가 연합함대
					if (first.GetType() == typeof(CombinedFleetViewModel))
					{
						// 연합함대에 포함
						if ((first as CombinedFleetViewModel).Source.Fleets.Any(x => x == fleet))
							this.SelectedFleet = first;
						else
							this.SelectedFleet = vm;
					}
					else this.SelectedFleet = vm;
				}
			}, false).AddTo(this.fleetListeners);

			return vm;
		}
	}
}
