using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Text;
using Grabacr07.KanColleViewer.Properties;

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
					case FleetSpeed.Fastest:
						return "초고속함대";
					case FleetSpeed.Faster:
						return "고속+함대";
					case FleetSpeed.Fast:
						return Resources.Fleets_Speed_Fast;
					case FleetSpeed.Low:
						return Resources.Fleets_Speed_Slow;

					case FleetSpeed.Hybrid_Faster:
						return "고속+혼성함대";
					case FleetSpeed.Hybrid_Fast:
						return "고속혼성함대";
					case FleetSpeed.Hybrid_Low:
						return "저속혼성함대";

					default:
						return "알수없음";
				}
			}
		}
		/*
		public string Speed => this.Source.Speed.IsMixed
			? $"速度混成艦隊 ({this.Source.Speed.Min.ToDisplayString()} ～ {this.Source.Speed.Max.ToDisplayString()})"
			: $"{this.Source.Speed.Min.ToDisplayString()}艦隊";
		*/

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
