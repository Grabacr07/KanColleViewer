using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum QuestState
	{
		/// <summary>
		/// 任務を遂行していません。
		/// </summary>
		None = 1,

		/// <summary>
		/// 任務を遂行中です。
		/// </summary>
		TakeOn = 2,

		/// <summary>
		/// 任務が完了しました。
		/// </summary>
		Accomplished = 3,
	}
}
