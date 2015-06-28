using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class FleetStateViewModel : ViewModel
	{
		public FleetState Source { get; }

		public string AverageLevel => this.Source.AverageLevel.ToString("#0.##");

		public string TotalLevel => this.Source.TotalLevel.ToString("###0");

		public string AirSuperiorityPotential => this.Source.AirSuperiorityPotential.ToString("##0");

		public string ViewRange => this.Source.ViewRange.ToString("##0.##");

		public string Speed
		{
			get
			{
				switch (this.Source.Speed)
				{
					case FleetSpeed.Fast:
						return Resources.Fleets_Speed_Fast;
					case FleetSpeed.Low:
						return Resources.Fleets_Speed_Slow;
					default:
						return "速度混成艦隊";
				}
			}
		}

		public HomeportViewModel Homeport { get; }

		public SortieViewModel Sortie { get; }


		public FleetStateViewModel(FleetState source)
		{
			this.Source = source;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			this.Sortie = new SortieViewModel(source);
			this.CompositeDisposable.Add(this.Sortie);

			this.Homeport = new HomeportViewModel(source);
			this.CompositeDisposable.Add(this.Homeport);
		}
	}
}
