using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 戦闘における艦隊陣形を示す識別子を取得します。
	/// </summary>
	public enum CombatFormation
	{
		Unknown,

		/// <summary>
		/// 単縦陣。
		/// </summary>
		LineAhead = 1,

		/// <summary>
		/// 複縦陣。
		/// </summary>
		DoubleLine = 2,

		/// <summary>
		/// 輪形陣。
		/// </summary>
		Diamond = 3,

		/// <summary>
		/// 梯形陣。
		/// </summary>
		Echelon = 4,

		/// <summary>
		/// 単横陣。
		/// </summary>
		LineAbreast = 5,
	}
}