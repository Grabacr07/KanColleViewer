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
		/// 1 回のみの任務。
		/// </summary>
		OneTime = 1,

		/// <summary>
		/// デイリー任務。
		/// </summary>
		Daily = 2,

		/// <summary>
		/// ウィークリー任務。
		/// </summary>
		Weekly = 3,

		/// <summary>
		/// 日付の一の位が3,7,0の日のみ出現する任務。
		/// </summary>
		X3X7X0 = 4,

		/// <summary>
		/// 日付の一の位が2,8の日のみ出現する任務。
		/// </summary>
		X2X8 = 5,

		/// <summary>
		/// マンスリー任務。
		/// </summary>
		Monthly = 6,
	}
}
