using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 工廠の状態を示す識別子を定義します。
	/// </summary>
	public enum BuildingDockState
	{
		/// <summary>
		/// ドックは未開放です。
		/// </summary>
		Locked = -1,

		/// <summary>
		/// ドックを使用可能です。
		/// </summary>
		Unlocked = 0,

		/// <summary>
		/// 新しい艦娘の建造中です。
		/// </summary>
		Building = 2,

		/// <summary>
		/// 新しい艦娘の建造が完了しています。
		/// </summary>
		Completed = 3,
	}
}
