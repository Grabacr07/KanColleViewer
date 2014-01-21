using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum FleetState
	{
		Empty,

		/// <summary>
		/// 出撃準備ができています。
		/// </summary>
		Ready,

		/// <summary>
		/// 艦隊は遠征中です。
		/// </summary>
		Expedition,

		/// <summary>
		/// 艦隊に入渠中の艦娘がいます。
		/// </summary>
		Repairing,
	}
}
