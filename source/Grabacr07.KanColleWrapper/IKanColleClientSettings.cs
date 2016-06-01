using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IKanColleClientSettings : INotifyPropertyChanged
	{
		/// <summary>
		/// 入渠完了と遠征帰還の通知における、通知短縮時間 (秒) を取得します。
		/// </summary>
		int NotificationShorteningTime { get; }

		/// <summary>
		/// 艦隊が再出撃可能と判断する基準となるコンディション値を取得します。
		/// </summary>
		int ReSortieCondition { get; }

		/// <summary>
		/// 索敵計算に使用するロジックを識別する文字列を取得します。
		/// </summary>
		string ViewRangeCalcType { get; }
		bool IsViewRangeCalcIncludeFirstFleet { get; }
		bool IsViewRangeCalcIncludeSecondFleet { get; }

		/// <summary>
		/// 艦隊ステータスにおいて、旗艦が工作艦かどうかを確認するかどうかを示す値を取得します。
		/// </summary>
		bool CheckFlagshipIsRepairShip { get; }
	}
}
