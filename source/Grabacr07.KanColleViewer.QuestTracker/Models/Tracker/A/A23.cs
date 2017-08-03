using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 제5항공전대를 편성하라!
	/// </summary>
	internal class A23 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 123;
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
					110, // 翔鶴
					111, // 瑞鶴
					288, // 翔鶴改
					112, // 瑞鶴改
					461, // 翔鶴改二
					462, // 瑞鶴改二
					466, // 翔鶴改二甲
					467, // 瑞鶴改二甲
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Info.Id)).Max(2)
							+ ships.Count(x=>x.Info.ShipType.Id==2).Max(2)
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

		public int GetProgress()
		{
			return count * 100 / max_count;
		}

		public string GetProgressText()
		{
			return count >= max_count ? "완료" : "쇼카쿠,즈이카쿠,구축 2척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
