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
	/// 센다이급 경순 자매 전 3척을 편성하라!
	/// </summary>
	internal class A14 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 3;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 109;
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
					54,  // 川内
					55,  // 神通
					56,  // 那珂
					158, // 川内改
					159, // 神通改
					160, // 那珂改
					222, // 川内改二
					223, // 神通改二
					224  // 那珂改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Info.Id))
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
			return count >= max_count ? "완료" : "센다이,진츠,나카 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
