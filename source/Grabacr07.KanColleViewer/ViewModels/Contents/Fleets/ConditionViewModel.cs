using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class ConditionViewModel : ViewModel
	{
		private readonly FleetCondition source;

		public string RejuvenateTime => this.source.RejuvenateTime?.LocalDateTime.ToString("MM/dd HH:mm") ?? "--/-- --:--";

		public string Remaining => this.source.Remaining.HasValue
			? $"{(int)this.source.Remaining.Value.TotalHours:D2}:{this.source.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public bool IsRejuvenating => this.source.IsRejuvenating;


		public ConditionViewModel(FleetCondition condition)
		{
			this.source = condition;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(condition, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));
		}
	}
}
