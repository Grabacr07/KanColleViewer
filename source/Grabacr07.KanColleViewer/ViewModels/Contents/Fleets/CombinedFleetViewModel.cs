using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class CombinedFleetViewModel : ItemViewModel
	{
		public CombinedFleet Source { get; }

		public string Name => this.Source?.Name.Replace(Environment.NewLine, " + ") ?? "";

		public FleetStateViewModel State { get; }

		public ViewModel QuickStateView => this.Source.State.Situation.HasFlag(FleetSituation.Sortie)
			? this.State.Sortie
			: this.State.Homeport as QuickStateViewViewModel;

        #region FleetViewModel과의 호환성
        public int Id => 1;
        public Expedition Expedition => this.Source?.Fleets[0].Expedition;
        public Visibility IsFirstFleet => Visibility.Collapsed;

        public Visibility vTotal => Visibility.Collapsed;
        public string TotalLv => "";
        public Visibility vFlag => Visibility.Collapsed;
        public string FlagLv => "";
        public Visibility vFlagType => Visibility.Collapsed;
        public string FlagType => "";
        public Visibility vNeed => Visibility.Collapsed;
        public string ShipTypeString => "";
        public Visibility vDrum => Visibility.Collapsed;
        public Visibility nDrum => Visibility.Collapsed;
        public Visibility vResource => Visibility.Collapsed;
        public Visibility vFuel => Visibility.Collapsed;
        public int nFuelLoss => 0;
        public Visibility vArmo => Visibility.Collapsed;
        public int nArmoLoss => 0;
        public List<int> ResultList => new List<int>();
        public int ExpeditionId { get; set; }
        public bool IsPassed => false;
        #endregion

        public CombinedFleetViewModel(CombinedFleet fleet)
		{
			this.Source = fleet;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				{ nameof(fleet.Name), (sender, args) => this.RaisePropertyChanged(nameof(this.Name)) },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ nameof(fleet.State.Situation), (sender, args) => this.RaisePropertyChanged(nameof(this.QuickStateView)) },
			});

			this.State = new FleetStateViewModel(fleet.State);
			this.CompositeDisposable.Add(this.State);
		}
	}
}
