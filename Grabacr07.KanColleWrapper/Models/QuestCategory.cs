using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 任務のカテゴリを示す識別子を定義します。
	/// </summary>
	public enum QuestCategory
	{
		/// <summary>
		/// 編成任務。
		/// </summary>
		Composition = 1,

		/// <summary>
		/// 出撃任務。
		/// </summary>
		Sortie = 2,

		/// <summary>
		/// 演習任務。
		/// </summary>
		Practice = 3,

		/// <summary>
		/// 遠征任務。
		/// </summary>
		Expeditions = 4,

		/// <summary>
		/// 補給/入渠任務。
		/// </summary>
		Supply = 5,

		/// <summary>
		/// 工廠任務。
		/// </summary>
		Building = 6,

		/// <summary>
		/// 改装任務。
		/// </summary>
		Remodelling = 7,
	}
}
