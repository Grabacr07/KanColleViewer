using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;

namespace Grabacr07.KanColleViewer.Models
{
	public class ExpeditionData
	{
		public int ID { get; set; }
		public ShipViewModel[] Ships { get; set; }

		public decimal Fuel { get; set; }
		public decimal Ammo { get; set; }
		public decimal Steel { get; set; }
		public decimal Bauxite { get; set; }

		public decimal ExpFuel => decimal.Round(this.Fuel * this.Rate);
		public decimal ExpAmmo => decimal.Round(this.Fuel * this.Rate);
		public decimal ExpSteel => decimal.Round(this.Fuel * this.Rate);
		public decimal ExpBauxite => decimal.Round(this.Fuel * this.Rate);

		public decimal ExpGreatFuel => decimal.Round(1.5m * this.Fuel * this.Rate);
		public decimal ExpGreatAmmo => decimal.Round(1.5m * this.Fuel * this.Rate);
		public decimal ExpGreatSteel => decimal.Round(1.5m * this.Fuel * this.Rate);
		public decimal ExpGreatBauxite => decimal.Round(1.5m * this.Fuel * this.Rate);

		public decimal Rate
		{
			get
			{
				var items = this.Ships.SelectMany(x => x.Ship.Slots).Select(x => x.Item);

				decimal result = 0.0m;
				result += items.Where(x => x.Info.Id == 68).Sum(x => 5 + 0.05m * x.Level); // 大発動艇
				result += items.Where(x => x.Info.Id == 166).Sum(x => 2 + 0.02m * x.Level); // 大発動艇(八九式中戦車＆陸戦隊)
				result += items.Where(x => x.Info.Id == 167).Sum(x => 1 + 0.01m * x.Level); // 特二式内火艇
				result = Math.Max(result, 20);

				result += items.Where(x => x.Info.Id == 193).Sum(x => 7 + 0.07m * x.Level); // 特大発動艇
				result = Math.Max(result, 22);
				return result / 100; // percent to rate
			}
		}
	}
}
