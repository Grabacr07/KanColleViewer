using System;
using System.Collections.Generic;
using System.Linq;
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

		public string MinAirSuperiorityPotential => this.Source.MinAirSuperiorityPotential.ToString("##0");

		public string MaxAirSuperiorityPotential => this.Source.MaxAirSuperiorityPotential.ToString("##0");

		public string ViewRange => (Math.Floor(this.Source.ViewRange * 100) / 100).ToString("##0.##");

		public string Speed => this.Source.Speed.IsMixed
			? $"速度混成艦隊 ({this.Source.Speed.Min.ToDisplayString()} ～ {this.Source.Speed.Max.ToDisplayString()})"
			: $"{this.Source.Speed.Min.ToDisplayString()}艦隊";

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
