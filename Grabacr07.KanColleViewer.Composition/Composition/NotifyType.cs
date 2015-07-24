using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// 通知の種類を表します。
	/// </summary>
	public enum NotifyType
	{
		/// <summary>
		/// その他。
		/// </summary>
		Other = 0,
		/// <summary>
		/// 遠征からの帰投。
		/// </summary>
		Expedition,
		/// <summary>
		/// 整備の完了。
		/// </summary>
		Repair,
		/// <summary>
		/// 建造の完了。
		/// </summary>
		Build,
		/// <summary>
		/// 疲労回復の完了。
		/// </summary>
		Rejuvenated,
	}
}
