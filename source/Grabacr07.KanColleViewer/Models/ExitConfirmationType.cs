using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// アプリケーションを終了するときの確認動作を示す識別子を定義します。
	/// </summary>
	public enum ExitConfirmationType
	{
		/// <summary>
		/// 終了は確認されません。
		/// </summary>
		None,

		/// <summary>
		/// 出撃中の場合のみ、終了を確認します。
		/// </summary>
		InSortieOnly,

		/// <summary>
		/// 常に終了を確認します。
		/// </summary>
		Always,
	}
}
