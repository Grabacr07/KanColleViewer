using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
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
