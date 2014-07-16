using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class SortieViewModel : ViewModel
	{
		public Fleet Fleet { get; private set; }

		public SortieViewModel(Fleet fleet)
		{
			this.Fleet = fleet;
		}
	}
}
