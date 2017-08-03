using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦隊の速度を表します。
	/// </summary>
	public enum FleetSpeed
	{
		Fastest,
		Faster,
		Fast,
		Low,

		Hybrid_Low, // 저속 혼성
		Hybrid_Fast, // 고속 혼성
		Hybrid_Faster, // 고속+ 혼성
	}

	public static class FleetSpeedExtension
	{
		public static bool IsFast(this FleetSpeed speed)
		{
			return (speed == FleetSpeed.Low || speed == FleetSpeed.Hybrid_Low)
				? false : true;
		}

		// Original branch added
		/*
		private readonly ShipSpeed[] speeds;

		public ShipSpeed? Min => this.speeds.Cast<ShipSpeed?>().Min();
		public ShipSpeed? Max => this.speeds.Cast<ShipSpeed?>().Max();
		public bool IsMixed => this.speeds.Distinct().Count() > 1;

		public FleetSpeed(ShipSpeed[] speeds)
		{
			this.speeds = speeds;
		}
		*/
	}
}
