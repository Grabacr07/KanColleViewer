using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 戦闘における交戦形態を示す識別子を取得します。
	/// </summary>
	public enum CombatEngagementForm
	{
		Unknown,

		/// <summary>
		/// 同航戦。
		/// </summary>
		Parallel = 1,

		/// <summary>
		/// 反抗戦
		/// </summary>
		HeadOn = 2,

		/// <summary>
		/// 丁字戦 (有利)。
		/// </summary>
		CrossingTAdvantage = 3,

		/// <summary>
		/// 丁字戦 (不利)。
		/// </summary>
		CrossingTDisadvantage = 4,
	}
}