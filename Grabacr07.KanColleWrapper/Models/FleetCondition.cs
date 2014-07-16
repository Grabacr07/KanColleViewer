using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class FleetCondition : TimerNotifier
	{
		private readonly Fleet fleet;
		private Ship[] ships;
		private bool notificated;
		private int minCondition;

		#region RejuvenateTime / IsRejuvenating 変更通知プロパティ

		private DateTimeOffset? _RejuvenateTime;

		/// <summary>
		/// 疲労回復の目安時間を取得します。
		/// </summary>
		public DateTimeOffset? RejuvenateTime
		{
			get { return this._RejuvenateTime; }
			private set
			{
				if (this._RejuvenateTime != value)
				{
					this._RejuvenateTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("IsRejuvenating");
				}
			}
		}

		/// <summary>
		/// 艦隊に編成されている艦娘の疲労を自然回復しているかどうかを示す値を取得します。
		/// </summary>
		public bool IsRejuvenating
		{
			get { return this.RejuvenateTime.HasValue; }
		}

		#endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		/// <summary>
		/// 疲労の回復が完了するまでの残り時間を取得します。1 秒ごとに更新されます。
		/// </summary>
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

		public event EventHandler<ConditionRejuvenatedEventArgs> Rejuvenated;

		internal FleetCondition(Fleet fleet)
		{
			this.fleet = fleet;
		}

		internal void Update(Ship[] s)
		{
			this.ships = s;

			var condition = this.ships.Min(x => x.Condition);
			if (condition != this.minCondition)
			{
				this.minCondition = condition;

				var rejuvnate = DateTimeOffset.Now; // 回復完了予測時刻

				while (condition < KanColleClient.Current.Settings.ReSortieCondition)
				{
					rejuvnate = rejuvnate.AddMinutes(3);
					condition += 3;
					if (condition > 49) condition = 49;
				}

				this.RejuvenateTime = rejuvnate <= DateTimeOffset.Now
					? (DateTimeOffset?)null
					: rejuvnate;
			}

			System.Diagnostics.Debug.WriteLine("FleetCondition.Update() - {0}", this.RejuvenateTime);
		}


		protected override void Tick()
		{
			base.Tick();

			if (this.RejuvenateTime.HasValue && this.fleet.State == FleetState.Homeport)
			{
				var remaining = this.RejuvenateTime.Value.Subtract(DateTimeOffset.Now);
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Rejuvenated != null && remaining.Ticks <= 0)
				{
					this.Rejuvenated(this, new ConditionRejuvenatedEventArgs(this.fleet.Name, 0));
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}
	}
}
