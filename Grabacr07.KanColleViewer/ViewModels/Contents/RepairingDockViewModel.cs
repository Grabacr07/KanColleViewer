using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class RepairingDockViewModel : ViewModel
	{
		private readonly RepairingDock source;

		public int Id => this.source.Id;

		public string Ship => this.source.Ship == null ? "----" : this.source.Ship.Info.Name;

		public string CompleteTime => this.source.CompleteTime?.LocalDateTime.ToString("MM/dd HH:mm") ?? "--/-- --:--:--";

		public string Remaining => this.source.Remaining.HasValue
			? $"{(int)this.source.Remaining.Value.TotalHours:D2}:{this.source.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public RepairingDockState State => this.source.State;

		public RepairingDockViewModel(RepairingDock source)
		{
			this.source = source;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));
		}
	}
}
