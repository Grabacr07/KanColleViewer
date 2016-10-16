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
	/// 수뢰전대를 편성하라!
	/// </summary>
	internal class A3 : NoSerializeTracker, ITracker
	{
		private int count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 103;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var fleet = homeport.Organization.Fleets[0];

				if (fleet.Ships[0].Info.ShipType.Id != 3) // 기함 경순
					count_1 = count_2 = 0;
				else
				{
					count_1 = fleet.Ships.Count(x => x.Info.ShipType.Id == 3).Max(1);
					count_2 = fleet.Ships.Count(x => x.Info.ShipType.Id == 2).Max(3);
				}

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count_1 = count_2 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public double GetProgress()
		{
			return (double)(count_1 + count_2) / (1 + 3) * 100;
		}

		public string GetProgressText()
		{
			return (count_1 + count_2) >= (1+3)
				? "완료"
				: "경순양함 기함, 경순 " + count_1.ToString() + " / 1척, 구축함 " + count_2.ToString() + " / 3척 편성";
		}
	}
}
