using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public abstract class QuickStateViewViewModel : ViewModel
	{
		public FleetState State { get; private set; }

		protected QuickStateViewViewModel(FleetState state)
		{
			this.State = state;
		}
	}
}