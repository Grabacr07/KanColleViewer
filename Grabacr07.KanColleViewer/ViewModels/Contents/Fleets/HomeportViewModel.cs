using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 母港で待機中の艦隊のステータスを表します。
	/// </summary>
	public class HomeportViewModel : ViewModel
	{
		public Fleet Fleet { get; private set; }

		public ConditionViewModel Condition { get; private set; }
		
		public HomeportViewModel(Fleet fleet)
		{
			this.Fleet = fleet;

			this.Condition = new ConditionViewModel(fleet.Condition);
			this.CompositeDisposable.Add(this.Condition);
		}
	}
}
