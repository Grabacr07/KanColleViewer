using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}
