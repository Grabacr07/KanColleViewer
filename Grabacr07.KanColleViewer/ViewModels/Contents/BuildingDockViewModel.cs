using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class BuildingDockViewModel : ViewModel
	{
		private readonly BuildingDock source;

		public int Id => this.source.Id;

	    public string Ship => this.source.Ship == null ? "----" : this.source.Ship.Name;

	    public string CompleteTime => this.source.CompleteTime.HasValue ? this.source.CompleteTime.Value.LocalDateTime.ToString("MM/dd HH:mm") : "--/-- --:--:--";

	    public string Remaining => this.source.Remaining.HasValue ? this.source.Remaining.Value.ToString(@"hh\:mm\:ss") : "--:--:--";

	    public BuildingDockState State => this.source.State;

	    public BuildingDockViewModel(BuildingDock source)
		{
			this.source = source;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));
		}
	}
}
