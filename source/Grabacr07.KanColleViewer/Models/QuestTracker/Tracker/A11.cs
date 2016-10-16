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
	/// 제2함대로 항모기동부대를 편성하라!
	/// </summary>
	internal class A11 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 117;
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
				var ships = homeport.Organization.Fleets[1].Ships;

				// 기함 경공모/정규공모/장갑공모
				if (ships[0].Info.ShipType.Id != 7 && ships[0].Info.ShipType.Id != 11 && ships[0].Info.ShipType.Id != 18)
					count = 0;
				else
					count = Math.Max(
						count,
						ships.Count(x => x.Info.ShipType.Id == 7 || x.Info.ShipType.Id == 11 || x.Info.ShipType.Id == 18).Max(1) +
						ships.Count(x => x.Info.ShipType.Id == 2).Max(3)
					);

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
			return count >= max_count ? "완료" : "2함대에 경공모/정규공모 기함 1척,구축함 3척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
