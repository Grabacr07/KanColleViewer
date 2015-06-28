using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class CombinedFleetViewModel : ItemViewModel
	{
		public CombinedFleet Source { get; }

		public string Name => this.Source.Name;

		public FleetStateViewModel State { get; }

		public ViewModel QuickStateView => this.Source.State.Situation.HasFlag(FleetSituation.Sortie)
			? this.State.Sortie
			: this.State.Homeport as QuickStateViewViewModel;

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
