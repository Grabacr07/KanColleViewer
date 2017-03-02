using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public sealed class AirBaseStateViewModel : ViewModel
	{
		public static AirBaseStateViewModel Instance { get; } = new AirBaseStateViewModel();

		private AirBaseStateViewModel() { }
	}
}
