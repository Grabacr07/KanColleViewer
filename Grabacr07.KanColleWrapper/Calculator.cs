﻿using System;
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
	}
}