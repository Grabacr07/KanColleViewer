using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

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
			// 함선 컨디션, 목표 출격 가능 컨디션
			var condition = ship.Condition;
			var goal = Math.Min(49, KanColleClient.Current.Settings.ReSortieCondition);

			if (condition >= goal)
			{
				this.RejuvenateTime = (DateTimeOffset?)null;
			}
			else
			{
				var rejuvenate = DateTimeOffset.Now; // 회복 완료 예상시각 기준은 현재부터

				// 3분 기준 컨디션 3이므로 공식 간소화
				var value = (goal - condition + 2) / 3 * 3; // 정수 나누기
				rejuvenate = rejuvenate.AddMinutes(value);

				/*
				// 기존 루프
				while (condition < goal)
				{
					rejuvenate = rejuvenate.AddMinutes(3);
					condition += 3;
					if (condition > 49) condition = 49;
				}
				*/

				this.RejuvenateTime = rejuvenate;
			}
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
