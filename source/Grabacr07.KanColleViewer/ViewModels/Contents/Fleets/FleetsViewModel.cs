using System;
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

		public FleetsViewModel()
		{
			KanColleClient.Current.Homeport.Organization
				.Subscribe(nameof(Organization.Fleets), this.UpdateFleets)
				.AddTo(this);
			Disposable
				.Create(() => this.fleetListeners?.Dispose())
				.AddTo(this);
		}

		public void ShowFleetWindow()
		{
			var fleetwd = new FleetWindowViewModel();
			var message = new TransitionMessage(fleetwd, TransitionMode.Normal, "FleetWindow.Show");
			this.Messenger.Raise(message);
		}


		private void UpdateFleets()
		{
			this.fleetListeners?.Dispose();
			this.fleetListeners = new MultipleDisposable();

			this.Fleets = KanColleClient.Current.Homeport.Organization.Fleets
				.Select(kvp => this.ToViewModel(kvp.Value))
				.ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}

		private FleetViewModel ToViewModel(Fleet fleet)
		{
			var vm = new FleetViewModel(fleet).AddTo(this.fleetListeners);
			fleet.Subscribe(nameof(Fleet.ShipsUpdated), () => { if (KanColleSettings.AutoFleetSelectWhenShipsChanged) this.SelectedFleet = vm; }, false).AddTo(this.fleetListeners);
			fleet.Subscribe(nameof(Fleet.IsInSortie), () => { if (KanColleSettings.AutoFleetSelectWhenSortie) this.SelectedFleet = vm; }, false).AddTo(this.fleetListeners);

			return vm;
		}
	}
}
