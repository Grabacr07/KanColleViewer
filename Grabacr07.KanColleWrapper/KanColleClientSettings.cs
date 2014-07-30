using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	[Serializable]
	public class KanColleClientSettings : NotificationObject
	{
		#region NotificationShorteningTime 変更通知プロパティ

		private int _NotificationShorteningTime = 40;

		/// <summary>
		/// 入渠完了と遠征帰還の通知における、通知短縮時間 (秒) を取得または設定します。
		/// </summary>
		public int NotificationShorteningTime
		{
			get { return this._NotificationShorteningTime; }
			set
			{
				if (this._NotificationShorteningTime != value)
				{
					this._NotificationShorteningTime = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReSortieCondition 変更通知プロパティ

		private int _ReSortieCondition = 40;

		/// <summary>
		/// 艦隊が再出撃可能と判断する基準となるコンディション値を取得または設定します。
		/// </summary>
		public int ReSortieCondition
		{
			get { return this._ReSortieCondition; }
			set
			{
				if (this._ReSortieCondition != value)
				{
					this._ReSortieCondition = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableLogging 変更通知プロパティ

		private bool _EnableLogging;

		public bool EnableLogging
		{
			get { return this._EnableLogging; }
			set
			{
				if (this._EnableLogging != value)
				{
					this._EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

	}
}
