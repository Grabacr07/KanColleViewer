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

        public static FleetDamages Parse(IEnumerable<int> damages)
        {
            if (damages == null) throw new ArgumentNullException();
            var arr = damages.ToArray();
            if (arr.Length != 6) throw new ArgumentException("艦隊ダメージ配列の長さは6である必要があります。");
            return new FleetDamages
            {
                Ship1 = arr[0],
                Ship2 = arr[1],
                Ship3 = arr[2],
                Ship4 = arr[3],
                Ship5 = arr[4],
                Ship6 = arr[5],
            };
        }
    }

    public static class FleetDamagesExtensions
    {
        public static FleetDamages ToFleetDamages(this IEnumerable<int> damages)
        {
            return FleetDamages.Parse(damages);
        }
    }
}
