using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 任務の種類を示す識別子を定義します。
	/// </summary>
	public enum QuestType
	{
		/// <summary>
		/// デイリー任務。
		/// </summary>
		Daily = 1,

		/// <summary>
		/// ウィークリー任務。
		/// </summary>
		Weekly = 2,

		/// <summary>
		/// マンスリー任務。
		/// </summary>
		Monthly = 3,

		/// <summary>
		/// 1 回のみの任務。
		/// </summary>
		OneTime = 4,

		/// <summary>
		/// その他。
		/// </summary>
		Other = 5,
	}
}
