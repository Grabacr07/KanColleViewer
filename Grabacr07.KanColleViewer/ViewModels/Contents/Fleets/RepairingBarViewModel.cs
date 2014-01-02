using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class RepairingBarViewModel : TimerViewModel
	{
		private readonly Fleet source;

		public RepairingBarViewModel(Fleet fleet)
		{
			this.source = fleet;
		}

		protected override string CreateMessage()
		{
			var dock = source.Ships
				.Join(KanColleClient.Current.Homeport.Repairyard.Docks.Values.Where(d => d.Ship != null), s => s.Id, d => d.Ship.Id, (s, d) => d)
				.OrderByDescending(d => d.CompleteTime)
				.FirstOrDefault();
			if (dock == null)
			{
				return "艦隊に入渠中の艦娘がいます。";
			}
			var remaining = dock.CompleteTime.Value.LocalDateTime - DateTimeOffset.Now - TimeSpan.FromMinutes(1.0);
			return string.Format(@"艦隊に入渠中の艦娘がいます。 完了時刻: {0:MM/dd HH\:mm} 完了まで: {1}:{2:mm\:ss}",
				dock.CompleteTime.Value.LocalDateTime, (int)remaining.TotalHours, remaining);
		}
	}
}
