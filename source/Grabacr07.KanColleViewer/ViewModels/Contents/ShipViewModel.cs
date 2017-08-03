using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipViewModel : ViewModel
	{
		public Ship Ship { get; }
		public ShipCondition Condition { get; }

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;
			this.Condition = new ShipCondition(ship);

			this.CompositeDisposable.Add(this.Condition);
		}
	}
}
