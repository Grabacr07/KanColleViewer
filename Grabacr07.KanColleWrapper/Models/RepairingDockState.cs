using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 入渠ドックの状態を示す識別子を定義します。
	/// </summary>
	public enum RepairingDockState
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
		/// 艦娘の修理中です。
		/// </summary>
		Repairing = 1,
	}
}
