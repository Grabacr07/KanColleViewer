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

		public string StatsToolTip
		{
			get
			{
				string addDetail = "";
				if (this.Ship.Info.NextRemodelingLevel != null)
					addDetail += string.Format("{0}: Lv. {1}\n", Resources.Stats_RemodelLevel, this.Ship.Info.NextRemodelingLevel);
				addDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Firepower, this.Ship.Firepower.Current, (this.Ship.Firepower.IsMax ? @"MAX" : "+" + (this.Ship.Firepower.Max - this.Ship.Firepower.Current).ToString()));
				addDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Torpedo, this.Ship.Torpedo.Current, (this.Ship.Torpedo.IsMax ? @"MAX" : "+" + (this.Ship.Torpedo.Max - this.Ship.Torpedo.Current).ToString()));
				addDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_AntiAir, this.Ship.AA.Current, (this.Ship.AA.IsMax ? @"MAX" : "+" + (this.Ship.AA.Max - this.Ship.AA.Current).ToString()));
				addDetail += string.Format("{0}: {1} ({2})\n", Resources.Stats_Armor, this.Ship.Armer.Current, (this.Ship.Armer.IsMax ? @"MAX" : "+" + (this.Ship.Armer.Max - this.Ship.Armer.Current).ToString()));
				addDetail += string.Format("{0}: {1} ({2})", Resources.Stats_Luck, this.Ship.Luck.Current, (this.Ship.Luck.IsMax ? @"MAX" : "+" + (this.Ship.Luck.Max - this.Ship.Luck.Current).ToString()));

				return addDetail;
			}
		}

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;
		}
	}
}
