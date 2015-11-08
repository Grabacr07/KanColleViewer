using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipViewModel : ViewModel
	{
		public Ship Ship { get; }

		public string RepairToolTip => 
			!((this.Ship.HP.Maximum - this.Ship.HP.Current) > 0) ? 
				"OK" : 
				string.Format(Resources.Ship_RepairDockToolTip, this.Ship.RepairTimeDock) 
				+ ((((this.Ship.HP.Current / (double)this.Ship.HP.Maximum) > 0.5) && this.Ship.RepairTimeFacility != this.Ship.RepairTimeDock) ? 
					"\n" + string.Format(Resources.Ship_RepairFacilityToolTip, this.Ship.RepairTimeFacility) : 
					"");

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;
		}
	}
}
