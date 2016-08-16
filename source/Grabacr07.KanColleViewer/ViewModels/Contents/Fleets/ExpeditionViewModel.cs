using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class ExpeditionViewModel : ViewModel
	{
		private readonly Expedition source;

		public Mission Mission => this.source.Mission;

		public bool IsInExecution => this.source.IsInExecution;

		public string ReturnTime => this.source.ReturnTime?.LocalDateTime.ToString("MM/dd HH:mm") ?? "--/-- --:--";

        public bool Returned => (this.source.Remaining.HasValue ? this.source.Remaining.Value == TimeSpan.Zero : false);

		public string Remaining => this.source.Remaining.HasValue
			? $"{(int)this.source.Remaining.Value.TotalHours:D2}:{this.source.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public ExpeditionViewModel(Expedition expedition)
		{
			this.source = expedition;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(expedition, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));
		}
	}
}
