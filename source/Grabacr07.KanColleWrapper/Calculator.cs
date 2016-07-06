using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	internal static class Calculator
	{
		/// <summary>
		/// 촉접개시율을 계산
		/// </summary>
		/// <param name="ship"></param>
		/// <returns></returns>
		public static double CalcFirstEncounterPercent(this Ship ship)
		{
			if (ship.Info.IsAirCraft)
			{
				return ship.EquippedItems
				.Select(x => x.Item.CalcFirstEncounterPercent(x.Current))
				.Sum();
			}
			return 0;
		}
		/// <summary>
		/// 장비하고 있는 함재기중에 촉접 가능 함재기의 명중률 목록을 뽑아냅니다.
		/// </summary>
		/// <param name="ship"></param>
		/// <returns></returns>
		public static List<int> HitStatusList(this Ship ship)
		{
			if (ship.Info.IsAirCraft)
			{
				List<int> templist = new List<int>();
				for (int i = 0; i < ship.EquippedItems.Count(); i++)
				{
					if (ship.EquippedItems[i].Item.Info.IsSecondEncounter)
					{
						templist.Add(ship.EquippedItems[i].Item.Info.Hit);
					}
				}
				return templist;
			}

			return new List<int>();//항공모함이 아닌경우 빈 리스트를 반환
		}

		/// <summary>
		/// 촉접 개시율을 계산합니다. 이 값은 모두 합산되서 사용됩니다. 함상정찰기와 수상정찰기 한정
		/// </summary>
		/// <param name="slotItem"></param>
		/// <param name="onslot"></param>
		/// <returns></returns>
		private static double CalcFirstEncounterPercent(this SlotItem slotItem, int onslot)
		{
			if (slotItem.Info.IsFirstEncounter)
			{
				return slotItem.Info.ViewRange * 0.04 * Math.Sqrt(onslot);
			}

			return 0;
		}

		public static double CalcViewRange(this Fleet fleet)
		{
			return ViewRangeCalcLogic.Get(KanColleClient.Current.Settings.ViewRangeCalcType).Calc(new[] { fleet });
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
