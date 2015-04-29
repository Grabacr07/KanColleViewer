using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class ExpeditionViewModel : ViewModel
	{
		private readonly Expedition source;

		public Mission Mission
		{
			get { return this.source.Mission; }
		}

		public bool IsInExecution
		{
			get { return this.source.IsInExecution; }
		}

		public string ReturnTime
		{
			get
			{
				return this.source.ReturnTime.HasValue
					? this.source.ReturnTime.Value.LocalDateTime.ToString("MM/dd HH:mm")
					: "--/-- --:--";
			}
		}

		public string Remaining
		{
			get
			{
				return this.source.Remaining.HasValue
					? string.Format("{0:D2}:{1}",
						(int)this.source.Remaining.Value.TotalHours,
						this.source.Remaining.Value.ToString(@"mm\:ss"))
					: "--:--:--";
			}
		}

		public ExpeditionViewModel(Expedition expedition)
		{
			this.source = expedition;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(expedition, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));
		}
	}
}
