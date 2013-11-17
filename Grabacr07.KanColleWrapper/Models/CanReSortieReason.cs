using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦隊の再出撃に関するステータスを示す識別子を定義します。
	/// </summary>
	[Flags]
	public enum CanReSortieReason
	{
		/// <summary>
		/// 再出撃に際して問題がないことを表します。
		/// </summary>
		NoProblem = 0,

		/// <summary>
		/// 艦隊にダメージを受けている艦娘がいることを表します。
		/// </summary>
		Wounded = 0x1,

		/// <summary>
		/// 艦隊に資源が不十分な艦娘がいることを表します。
		/// </summary>
		LackForResources = 0x2,

		/// <summary>
		/// 艦隊に疲労している艦娘がいることを表します。
		/// </summary>
		BadCondition = 0x4,
	}

}
