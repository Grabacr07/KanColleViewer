using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipViewModel : ViewModel
	{
		public Ship Ship { get; private set; }

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;
		}
	}
}
