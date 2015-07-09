using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	[Flags]
	public enum FleetSituation
	{
		/// <summary>
		/// 艦隊に艦娘が編成されていません。
		/// </summary>
		Empty = 0,

		/// <summary>
		/// 艦隊は母港で待機中です。
		/// </summary>
		Homeport = 1,

		/// <summary>
		/// この艦隊は連合艦隊です。
		/// </summary>
		Combined = 1 << 1,

		/// <summary>
		/// 艦隊は出撃中です。
		/// </summary>
		Sortie = 1 << 2,

		/// <summary>
		/// 艦隊は遠征中です。
		/// </summary>
		Expedition = 1 << 3,

		/// <summary>
		/// 艦隊に大破した艦娘がいます。
		/// </summary>
		HeavilyDamaged = 1 << 4,

		/// <summary>
		/// 艦隊に完全に補給されていない艦娘がいます。
		/// </summary>
		InShortSupply = 1 << 5,

		/// <summary>
		/// 艦隊に入渠中の艦娘がいます。
		/// </summary>
		Repairing = 1 << 6,
	}
}
