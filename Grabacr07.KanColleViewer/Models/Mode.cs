
namespace Grabacr07.KanColleViewer.Models
{
	public enum Mode
	{
		/// <summary>
		/// 艦これが起動されていません。
		/// </summary>
		NotStarted,

		/// <summary>
		/// 艦これが起動されています。
		/// </summary>
		Started,

		/// <summary>
		/// 艦これが起動されており、艦隊が出撃中です。
		/// </summary>
		InSortie,
		/// <summary>
		/// When any ship in the fleet is in critical condition.
		/// </summary>
		CriticalCondition,
	}
}
