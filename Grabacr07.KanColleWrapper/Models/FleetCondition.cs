using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class FleetCondition : TimerNotificator
	{
		private bool notificated;
		private int minCondition;

		#region IsInRecovering 変更通知プロパティ

		private bool _IsInRecovering;

		/// <summary>
		/// 疲労回復中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInRecovering
		{
			get { return this._IsInRecovering; }
			private set
			{
				if (this._IsInRecovering != value)
				{
					this._IsInRecovering = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RevivalTime 変更通知プロパティ

		private DateTimeOffset? _RevivalTime;

		/// <summary>
		/// 艦隊のすべての艦娘の疲労度が回復するおおよその時刻を取得します。
		/// </summary>
		public DateTimeOffset? RevivalTime
		{
			get { return this._RevivalTime; }
			private set
			{
				if (this._RevivalTime != value)
				{
					this._RevivalTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			private set
			{
				if (this._Remaining != value)
				{
					this._Remaining = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// 艦隊のすべての艦娘の疲労が回復したときに発生します。
		/// </summary>
		public event EventHandler Recovered;

		internal void Update(Ship[] ships)
		{
			if (ships.Length == 0)
			{
				this.RevivalTime = null;
				this.IsInRecovering = false;
				this.UpdateCore();
				return;
			}

			var min = ships.Select(x => x.Condition).Min();
			if (min == this.minCondition) return; // 最小値が同じなら、前回設定したカウントダウンをそのまま引き継ぐ
			
			if (min < 40)
			{
				this.RevivalTime = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(40 - min));
				this.IsInRecovering = true;
			}
			else
			{
				this.RevivalTime = null;
				this.IsInRecovering = false;
			}
			this.minCondition = min;
		}

		private void UpdateCore()
		{
			if (this.RevivalTime.HasValue)
			{
				var remaining = this.RevivalTime.Value - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Recovered != null && remaining.Ticks <= 0)
				{
					this.Recovered(this, new EventArgs());
					this.notificated = true;
					this.IsInRecovering = false;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
	}
}
