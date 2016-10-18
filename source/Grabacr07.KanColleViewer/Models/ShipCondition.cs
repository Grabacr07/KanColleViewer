using System;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Models
{
	public class ShipCondition : TimerNotifier
	{
		private DateTimeOffset? RejuvenateTime { get; }

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
					this.RaisePropertyChanged("ConditionText");
				}
			}
		}

		public string ConditionText => this.Remaining.HasValue
			? $"{(int)this.Remaining.Value.TotalHours:D2}:{this.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public ShipCondition(Ship ship)
		{
			var condition = ship.Condition;
			var rejuvenate = DateTimeOffset.Now; // 回復予想時刻

			while (condition < Math.Min(49, KanColleClient.Current.Settings.ReSortieCondition))
			{
				rejuvenate = rejuvenate.AddMinutes(3);
				condition += 3;
				if (condition > 49) condition = 49;
			}
			this.RejuvenateTime = rejuvenate <= DateTimeOffset.Now
				? (DateTimeOffset?)null
				: rejuvenate;
		}

		protected override void Tick()
		{
			base.Tick();

			if (this.RejuvenateTime.HasValue)
			{
				var remaining = this.RejuvenateTime.Value.Subtract(DateTimeOffset.Now);
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;
			}
			else
			{
				this.Remaining = null;
			}
		}
	}
}
