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

		public decimal ExpFuel => Calculate(this.Fuel);
		public decimal ExpAmmo => Calculate(this.Ammo);
		public decimal ExpSteel => Calculate(this.Steel);
		public decimal ExpBauxite => Calculate(this.Bauxite);

		public decimal ExpGreatFuel => Calculate(this.Fuel, 1.5m);
		public decimal ExpGreatAmmo => Calculate(this.Ammo, 1.5m);
		public decimal ExpGreatSteel => Calculate(this.Steel, 1.5m);
		public decimal ExpGreatBauxite => Calculate(this.Bauxite, 1.5m);

		/// <summary>
		/// 대발계 장비 보정치를 포함한 예상 자원량 계산
		/// </summary>
		/// <param name="value">원본 취득량</param>
		/// <param name="additional">성공 1.0, 대성공 1.5</param>
		/// <returns></returns>
		public decimal Calculate(decimal value, decimal additional = 1.0m)
		{
			// 계산공식 수정
			// http://gall.dcinside.com/board/view/?id=kancolle&no=4857830

			int[] itemtable = new int[]
			{
				68,  // 大発動艇 (대발동정)
				166, // 大発動艇(八九式中戦車＆陸戦隊) (중전차)
				167, // 特二式内火艇 (내화정)
				193  // 特大発動艇 (특대발동정)
			};
			var items = this.Ships?.SelectMany(x => x.Ship.Slots)
				.Select(x => x.Item)
				.Where(x => itemtable.Contains(x.Info.Id));

			var kinu_kai_ni = this.Ships?.Count(x => x.Ship.Info.Id == 487) ?? 0; // 키누改2
			var daihatsu = items.Count(x => x.Info.Id == 68); // 대발동정
			var tokudaihatsu = items.Count(x => x.Info.Id == 193); // 특대발동정

			/// correction_A: 대발계 보정
			/// correction_B: 특대발 보정
			decimal correction_A = 0.0m, correction_B = 0.0m, levelAvrg;

			levelAvrg = items.Count() == 0 ? 0
				: (decimal)items.Average(x => x.Level); // 대발계 장비 개수 평균치

			#region 대발계 보정 계산
			correction_A += kinu_kai_ni * 5; // 키누改2 는 대발동정처럼 작동한다?
			correction_A += items.Where(x => x.Info.Id == 68).Sum(x => 5);  // 大発動艇 (대발동정)
			correction_A += items.Where(x => x.Info.Id == 193).Sum(x => 5); // 特大発動艇 (특대발동정)
			correction_A += items.Where(x => x.Info.Id == 166).Sum(x => 2); // 大発動艇(八九式中戦車＆陸戦隊) (중전차)
			correction_A += items.Where(x => x.Info.Id == 167).Sum(x => 1); // 特二式内火艇 (내화정)
			correction_A = Math.Min(correction_A, 20) / 100.0m;
			#endregion

			#region 특대발 보정 계산
			if (tokudaihatsu <= 2)
				correction_B = 0.02m * tokudaihatsu;
			else if (tokudaihatsu == 3)
			{
				if (daihatsu == 0)
					correction_B = 0.05m;
				else if (daihatsu == 1)
					correction_B = 0.05m;
				else if (daihatsu == 2)
					correction_B = 0.052m;
				else if (daihatsu == 3)
					correction_B = 0.054m;
				else
					correction_B = 0.054m;
			}
			else
			{
				if (daihatsu == 0)
					correction_B = 0.054m;
				else if (daihatsu == 1)
					correction_B = 0.056m;
				else if (daihatsu == 2)
					correction_B = 0.058m;
				else if (daihatsu == 3)
					correction_B = 0.059m;
				else
					correction_B = 0.06m;
			}
			#endregion

			return decimal.Floor(
				(value * (1.0m + correction_A + (0.01m * correction_A * levelAvrg)) * additional)
				+ (value * correction_B * additional)
			);
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
