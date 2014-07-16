using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum FleetState
	{
		/// <summary>
		/// 艦隊に艦娘が編成されていません。
		/// </summary>
		Empty,

		/// <summary>
		/// 艦隊は母港で待機中です。
		/// </summary>
		Homeport,

		/// <summary>
		/// 艦隊は出撃中です。
		/// </summary>
		Sortie,

		/// <summary>
		/// 艦隊は遠征中です。
		/// </summary>
		Expedition,
	}
}
