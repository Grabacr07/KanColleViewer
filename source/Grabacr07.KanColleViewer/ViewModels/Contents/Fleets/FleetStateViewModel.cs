using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Text;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class FleetStateViewModel : ViewModel
	{
		public FleetState Source { get; }

		public string AverageLevel => this.Source.AverageLevel.ToString("#0.##");
		public string TotalLevel => this.Source.TotalLevel.ToString("###0");

		public string MinAirSuperiorityPotential => this.Source.MinAirSuperiorityPotential.ToString("##0");
		public string MaxAirSuperiorityPotential => this.Source.MaxAirSuperiorityPotential.ToString("##0");

		public string EncounterPercent => this.Source.EncounterPercent.ToString("##0.##%");
		public string PartEncounterPercent
		{
			get
			{
				StringBuilder stbr = new StringBuilder();
				stbr.Append("촉접개시율: "+this.Source.FirstEncounter.ToString("##0.##%")+" ");
				foreach (var item in this.Source.PartEncounterPercent)
				{
					stbr.Append("명중률" + item.Hit + ": " + Math.Round(item.SecondEncounter * 100, 1) + "% ");
				}

				return stbr.ToString();
			}
		}
		
		public string ViewRange => (Math.Floor(this.Source.ViewRange * 100) / 100).ToString("##0.##");

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
						return "속도혼성함대";
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
