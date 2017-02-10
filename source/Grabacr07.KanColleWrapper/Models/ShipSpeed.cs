using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 速力を示す識別子を定義します。
	/// </summary>
	public enum ShipSpeed
	{
		/// <summary>
		/// 不動。(基地等)
		/// </summary>
		Immovable = 0,

		/// <summary>
		/// 低速。
		/// </summary>
		Slow = 5,

		/// <summary>
		/// 高速。
		/// </summary>
		Fast = 10,

		/// <summary>
		/// 高速＋。
		/// </summary>
		Faster = 15,

		/// <summary>
		/// 最速。
		/// </summary>
		Fastest = 20,
	}

	public static class ShipSpeedConverter
	{
		// ReSharper disable once InconsistentNaming
		public static ShipSpeed FromInt32(int api_soku)
		{
			if (api_soku > 15)
			{
				return ShipSpeed.Fastest;
			}
			if (api_soku > 10)
			{
				return ShipSpeed.Faster;
			}
			if (api_soku > 5)
			{
				return ShipSpeed.Fast;
			}
			if (api_soku > 0)
			{
				return ShipSpeed.Slow;
			}

			return ShipSpeed.Immovable;
		}
	}
}
