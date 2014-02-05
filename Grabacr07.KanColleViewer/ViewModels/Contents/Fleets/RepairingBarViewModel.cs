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
				return Properties.Resources.MessageBar_Repairing_Null;
			}
			var remaining = dock.CompleteTime.Value.LocalDateTime - DateTimeOffset.Now - TimeSpan.FromMinutes(1.0);
			return string.Format(@Properties.Resources.MessageBar_Repairing,
				dock.CompleteTime.Value.LocalDateTime, (int)remaining.TotalHours, remaining);
		}
	}
}
