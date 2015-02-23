using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	internal static class Calculator
	{
		/// <summary>
		/// 装備と搭載数を指定して、スロット単位の制空能力を計算します。
		/// </summary>
		/// <param name="slotItem">対空能力を持つ装備。</param>
		/// <param name="onslot">搭載数。</param>
		/// <returns></returns>
		public static int CalcAirSuperiorityPotential(this SlotItem slotItem, int onslot)
		{
			if (slotItem.Info.IsAirSuperiorityFighter)
			{
				return (int)(slotItem.Info.AA * Math.Sqrt(onslot));
			}

			return 0;
		}

		/// <summary>
		/// 指定した艦の制空能力を計算します。
		/// </summary>
		public static int CalcAirSuperiorityPotential(this Ship ship)
		{
			return ship.EquippedSlots.Select(x => x.Item.CalcAirSuperiorityPotential(x.Current)).Sum();
		}

		public static double CalcViewRange(this Fleet fleet)
		{
			return ViewRangeCalcLogic.Get(KanColleClient.Current.Settings.ViewRangeCalcType).Calc(fleet.Ships);
		}

		public static bool IsHeavilyDamage(this LimitedValue hp)
		{
			return (hp.Current / (double)hp.Maximum) <= 0.25;
		}

		/// <summary>
		/// 現在のシーケンスから護衛退避した艦娘を除きます。
		/// </summary>
		public static IEnumerable<Ship> WithoutEvacuated(this IEnumerable<Ship> ships)
		{
			return ships.Where(ship => !ship.Situation.HasFlag(ShipSituation.Evacuation) && !ship.Situation.HasFlag(ShipSituation.Tow));
		}
	}
}