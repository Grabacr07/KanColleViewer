using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleInfoPlugin.Models
{
    /// <summary>
    /// 1艦隊分のダメージ一覧
    /// </summary>
    public class FleetDamages
    {
        public int Ship1 { get; set; }
		public int Ship2 { get; set; }
		public int Ship3 { get; set; }
		public int Ship4 { get; set; }
		public int Ship5 { get; set; }
		public int Ship6 { get; set; }

		public int[] ToArray()
        {
            return new[]
            {
                this.Ship1,
                this.Ship2,
                this.Ship3,
                this.Ship4,
                this.Ship5,
                this.Ship6,
            };
        }

        public static FleetDamages Parse(IEnumerable<decimal> damages)
        {
            if (damages == null) throw new ArgumentNullException();

			var arr = damages.ToArray();

			List<int> temp=new List<int>();
			for (int i = 0; i < arr.Count(); i++)
			{
				temp.Add(Convert.ToInt32(arr[i]));
			}


			var Intarr = temp.ToArray();

			if (Intarr.Length != 6) throw new ArgumentException("艦隊ダメージ配列の長さは6である必要があります。");
            return new FleetDamages
            {
				Ship1 = Intarr[0],
				Ship2 = Intarr[1],
				Ship3 = Intarr[2],
				Ship4 = Intarr[3],
				Ship5 = Intarr[4],
				Ship6 = Intarr[5],
            };
        }
    }

    public static class FleetDamagesExtensions
    {
        public static FleetDamages ToFleetDamages(this IEnumerable<decimal> damages)
        {
            return FleetDamages.Parse(damages);
        }
		public static FleetDamages ToFleetDamages(this IEnumerable<int> damages)
		{
			var arr = damages.ToArray();

			List<decimal> temp = new List<decimal>();
			for (int i = 0; i < arr.Count(); i++)
			{
				temp.Add(Convert.ToDecimal(arr[i]));
			}
			return FleetDamages.Parse(temp);
		}
    }
}
