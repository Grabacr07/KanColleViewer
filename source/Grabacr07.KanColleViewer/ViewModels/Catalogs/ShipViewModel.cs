using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipViewModel : ViewModel
	{
		public int Index { get; }

		public Ship Ship { get; }

		public SallyArea SallyArea { get; }

		public ShipViewModel(int index, Ship ship, SallyArea sallyArea = null)
		{
			this.Index = index;
			this.Ship = ship;
			if (sallyArea != null) this.SallyArea = sallyArea ?? SallyArea.Default;
		}
	}
}
