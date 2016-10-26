using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;
using Grabacr07.KanColleViewer.ViewModels.Contents;

namespace Grabacr07.KanColleViewer.Models
{
	public class ExpeditionResultData : ViewModel
	{
		public int ID { get; set; }
		public ShipViewModel[] Ships { get; set; }

		public decimal Fuel { get; set; }
		public decimal Ammo { get; set; }
		public decimal Steel { get; set; }
		public decimal Bauxite { get; set; }

		public decimal ExpFuel => decimal.Round(this.Fuel * this.Rate);
		public decimal ExpAmmo => decimal.Round(this.Ammo * this.Rate);
		public decimal ExpSteel => decimal.Round(this.Steel * this.Rate);
		public decimal ExpBauxite => decimal.Round(this.Bauxite * this.Rate);

		public decimal ExpGreatFuel => decimal.Round(1.5m * this.Fuel * this.Rate);
		public decimal ExpGreatAmmo => decimal.Round(1.5m * this.Ammo * this.Rate);
		public decimal ExpGreatSteel => decimal.Round(1.5m * this.Steel * this.Rate);
		public decimal ExpGreatBauxite => decimal.Round(1.5m * this.Bauxite * this.Rate);

		public decimal Rate
		{
			get
			{
				var items = this.Ships?.SelectMany(x => x.Ship.Slots).Select(x => x.Item);

				decimal result = 0.0m, max = 0.0m;
				result += items.Where(x => x.Info.Id == 68).Sum(x => 5 + 0.05m * x.Level); // 大発動艇
				result += items.Where(x => x.Info.Id == 166).Sum(x => 2 + 0.02m * x.Level); // 大発動艇(八九式中戦車＆陸戦隊)
				result += items.Where(x => x.Info.Id == 167).Sum(x => 1 + 0.01m * x.Level); // 特二式内火艇
				result = Math.Min(result, 20);

				result += items.Where(x => x.Info.Id == 193).Sum(x => 7 + 0.07m * x.Level); // 特大発動艇

				max = items.Count(x => x.Info.Id == 193) + items.Where(x => x.Info.Id == 193).Sum(x => x.Level * 0.1m);
				result = Math.Min(result, 20 + max); // +2% per 特大発動艇, +0.1% per Improvement
				return 1 + result / 100; // percent to rate
			}
		}

		public ExpeditionResultData(int expeditionId, ShipViewModel[] Ships)
		{
			var eid = expeditionId;

			this.ID = eid;
			this.Ships = Ships;
			this.Fuel = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Fuel", eid));
			this.Ammo = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Armo", eid));
			this.Steel = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Metal", eid));
			this.Bauxite = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Bo", eid));

			this.RefreshProperties();
		}

		public void RefreshProperties()
		{
			this.RaisePropertyChanged(nameof(this.Fuel));
			this.RaisePropertyChanged(nameof(this.Ammo));
			this.RaisePropertyChanged(nameof(this.Steel));
			this.RaisePropertyChanged(nameof(this.Bauxite));
			this.RaisePropertyChanged(nameof(this.Rate));
			this.RaisePropertyChanged(nameof(this.ExpFuel));
			this.RaisePropertyChanged(nameof(this.ExpAmmo));
			this.RaisePropertyChanged(nameof(this.ExpSteel));
			this.RaisePropertyChanged(nameof(this.ExpBauxite));
			this.RaisePropertyChanged(nameof(this.ExpGreatFuel));
			this.RaisePropertyChanged(nameof(this.ExpGreatAmmo));
			this.RaisePropertyChanged(nameof(this.ExpGreatSteel));
			this.RaisePropertyChanged(nameof(this.ExpGreatBauxite));
		}

		private decimal ConvertToDecimal(string context)
		{
			if (context == string.Empty) return 0;
			return Convert.ToDecimal(context);
		}
	}
}
