using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Fleets
{
	/// <summary>
	/// 艦隊の再出撃に関する情報を提供します。
	/// </summary>
	public class ReSortieBarViewModel : TimerViewModel
	{
		[Flags]
		public enum CanReSortieReason
		{
			/// <summary>
			/// 再出撃に際して問題がないことを表します。
			/// </summary>
			NoProblem = 0,
			/// <summary>
			/// 艦隊にダメージを受けている艦娘がいることを表します。
			/// </summary>
			Wounded = 0x1,
			/// <summary>
			/// 艦隊に資源が不十分な艦娘がいることを表します。
			/// </summary>
			LackForResources = 0x2,
			/// <summary>
			/// 艦隊に疲労している艦娘がいることを表します。
			/// </summary>
			BadCondition = 0x4,
		}

		private readonly Fleet source;

		private DateTimeOffset period;

		#region Reason 変更通知プロパティ

		private CanReSortieReason _Reason;

		/// <summary>
		/// 艦隊が再出撃可能かどうかを示す値を取得します。
		/// </summary>
		public CanReSortieReason Reason
		{
			get
			{
				return this._Reason;
			}
			private set
			{
				if (this._Reason != value)
				{
					this._Reason = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(() => CanReSortie);
				}
			}
		}

		#endregion

		public bool CanReSortie
		{
			get { return this.Reason == CanReSortieReason.NoProblem; }
		}

		public ReSortieBarViewModel(Fleet fleet)
		{
			this.source = fleet;
		}

		protected override string CreateMessage()
		{
			this.Update();

			if (this.CanReSortie)
			{
				return "出撃可能！";
			}

			var remaining = period - DateTimeOffset.Now;
			if (this.Reason.HasFlag(CanReSortieReason.BadCondition) && remaining <= TimeSpan.Zero)
			{
				this.Reason ^= CanReSortieReason.BadCondition;
			}

			var message = new StringBuilder();

			if (this.Reason.HasFlag(CanReSortieReason.Wounded))
			{
				message.Append("艦隊に中破以上の艦娘がいます。");
			}
			if (this.Reason.HasFlag(CanReSortieReason.LackForResources))
			{
				message.Append("艦隊の補給が不十分です。");
			}
			if (this.Reason.HasFlag(CanReSortieReason.BadCondition))
			{
				message.Append("艦隊に疲労中の艦娘がいます。");
			}

			if (remaining > TimeSpan.Zero)
			{
				message.AppendFormat("再出撃までの目安: {0}", remaining.ToString("mm\\:ss"));
			}

			return message.ToString();
		}
		private void Update()
		{
			var reason = CanReSortieReason.NoProblem;

			if (this.source.GetShips().Any(s => (s.HP.Current / (double) s.HP.Maximum) <= 0.5))
			{
				reason |= CanReSortieReason.Wounded;
			}

			if (this.source.GetShips().Any(s => s.Fuel.Current < s.Fuel.Maximum || s.Bull.Current < s.Bull.Maximum))
			{
				reason |= CanReSortieReason.LackForResources;
			}

			var minCondition = source.GetShips().Min(s => s.Condition);
			if (minCondition < 40)
			{
				if (!this.Reason.HasFlag(CanReSortieReason.BadCondition))
				{
					// 疲労状態に遷移したので、再出撃目安時刻を更新
					this.period = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(40 - minCondition));
				}
				reason |= CanReSortieReason.BadCondition;
			}

			this.Reason = reason;
		}
	}
}
