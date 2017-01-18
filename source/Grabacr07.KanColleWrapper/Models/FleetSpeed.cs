using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum FleetSpeed
	{
		SuperFast,
		FastPlus,
		Fast,
		Low,

		Hybrid_Low, // 저속 혼성
		Hybrid_Fast, // 고속 혼성
		Hybrid_FastPlus, // 고속+ 혼성
	}

	public static class FleetSpeedExtension
	{
		public static bool IsFast(this FleetSpeed speed)
		{
			return (speed == FleetSpeed.Low || speed == FleetSpeed.Hybrid_Low)
				? false : true;
		}
	}
}