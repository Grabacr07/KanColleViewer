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
				return "함대에 입거중인 칸무스가 있습니다.";
			}
			var remaining = dock.CompleteTime.Value.LocalDateTime - DateTimeOffset.Now - TimeSpan.FromMinutes(1.0);
            return string.Format(@"함대에 입거중인 칸무스가 있습니다. 완료시각: {0:MM/dd HH\:mm} 완료까지: {1}:{2:mm\:ss}",
				dock.CompleteTime.Value.LocalDateTime, (int)remaining.TotalHours, remaining);
		}
	}
}
