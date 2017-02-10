using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦隊の速度を表します。
	/// </summary>
	public class FleetSpeed
	{
		private readonly ShipSpeed[] speeds;

		/// <summary>
		/// 最も遅い艦を基準とした艦隊の速度を示す識別子を取得します。
		/// </summary>
		public ShipSpeed? Min => this.speeds.Cast<ShipSpeed?>().Min();

		public ShipSpeed? Max => this.speeds.Cast<ShipSpeed?>().Max();
		
		public bool IsMixed => this.speeds.Distinct().Count() > 1;

		public FleetSpeed(ShipSpeed[] speeds)
		{
			this.speeds = speeds;
		}
	}
}
