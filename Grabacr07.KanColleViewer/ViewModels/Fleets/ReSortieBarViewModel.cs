using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly DateTimeOffset period;

		#region CanReSortie 変更通知プロパティ

		private bool _CanReSortie;

		/// <summary>
		/// 艦隊が再出撃可能かどうかを示す値を取得します。
		/// </summary>
		public bool CanReSortie
		{
			get { return this._CanReSortie; }
			private set
			{
				if (this._CanReSortie != value)
				{
					this._CanReSortie = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ReSortieBarViewModel(Fleet fleet)
		{
			if (fleet.GetShips().Any(s => s.Fuel.Current < s.Fuel.Maximum || s.Bull.Current < s.Bull.Maximum))
			{
				this.CanReSortie = false;
				return;
			}

			var minCondition = fleet.GetShips().Min(s => s.Condition);
			if (minCondition <= 40)
			{
				this.CanReSortie = false;
				this.period = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(40 - minCondition));
				return;
			}

			this.CanReSortie = true;
		}

		protected override string CreateMessage()
		{
			if (this.CanReSortie)
			{
				return "出撃可能！";
			}

			if (period == default(DateTimeOffset))
			{
				return "艦隊の補給が不十分です。";
				
			}

			var remaining = period - DateTimeOffset.Now;
			if (remaining.Ticks <= 0)
			{
				this.CanReSortie = true;
				return "出撃可能！";
			}
			else
			{
				return string.Format("艦隊に疲労中の艦娘がいます。再出撃までの目安: {0}", remaining.ToString("mm\\:ss"));
			}
		}
	}
}
