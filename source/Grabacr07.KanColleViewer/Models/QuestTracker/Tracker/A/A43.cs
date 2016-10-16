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
	/// 전함을 주력으로 한 수상타격부대를 편성하라!
	/// </summary>
	internal class A43 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 145;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var shipTable = new int[]
				{
					131, // 大和
					143, // 武蔵
					80,  // 長門
					81,  // 陸奥
					26,  // 扶桑
					27,  // 山城
					77,  // 伊勢
					87,  // 日向
					136, // 大和改
					148, // 武蔵改
					275, // 長門改
					276, // 陸奥改
					286, // 扶桑改
					287, // 山城改
					82,  // 伊勢改
					88,  // 日向改
					411, // 扶桑改二
					412, // 山城改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Id)).Max(3)
							+ ships.Count(x => x.Info.ShipType.Id == 3).Max(1)
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
			return count >= max_count ? "완료" : "야마토급,나가토급,이세급,후소급 중 3척,경순 1척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
