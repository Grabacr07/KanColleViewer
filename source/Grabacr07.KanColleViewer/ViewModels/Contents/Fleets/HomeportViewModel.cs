using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 母港で待機中の艦隊のステータスを表します。
	/// </summary>
	public class HomeportViewModel : QuickStateViewViewModel
	{
		// QuickStateView は ContentControl に対し型ごとの DataTemplate を適用する形で実現するので
		// 状況に応じた型がそれぞれ必要。これはその 1 つ。

		public ConditionViewModel Condition { get; }

		public HomeportViewModel(FleetState state)
			: base(state)
		{
			this.Condition = new ConditionViewModel(state.Condition);
			this.CompositeDisposable.Add(this.Condition);
		}
	}
}
