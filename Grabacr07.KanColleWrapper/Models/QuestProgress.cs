using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum QuestProgress
	{
		/// <summary>
		/// 進捗ダメです。
		/// </summary>
		None = 0,

		/// <summary>
		/// 50 % 以上達成。
		/// </summary>
		Progress50 = 1,

		/// <summary>
		/// 80 % 以上達成。
		/// </summary>
		Progress80 = 2,
	}
}
