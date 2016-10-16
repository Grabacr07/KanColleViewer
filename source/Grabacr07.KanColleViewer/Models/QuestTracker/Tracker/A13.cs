using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Tracker
{
	/// <summary>
	/// 전함과 중순으로 이루어진 주력함대를 편성하라!
	/// </summary>
	internal class A13 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 3;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 113;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => x.Info.ShipType.Id == 8 || x.Info.ShipType.Id == 9).Max(1)
							+ ships.Count(x => x.Info.ShipType.Id == 5).Max(2)
					);
				}

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public double GetProgress()
		{
			return (double)count / max_count * 100;
		}

		public string GetProgressText()
		{
			return count >= max_count ? "완료" : "전함 1척과 중순양함 2척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
