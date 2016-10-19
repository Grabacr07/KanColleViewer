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
	/// 미카와 함대를 편성하라!
	/// </summary>
	internal class A20 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 6;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 119;
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
					59,  // 古鷹
					60,  // 加古
					61,  // 青葉
					69,  // 鳥海
					51,  // 天龍
					262, // 古鷹改
					263, // 加古改
					264, // 青葉改
					272, // 鳥海改
					213, // 天龍改
					416, // 古鷹改二
					417, // 加古改二
					427, // 鳥海改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					var y = ships.Count(x => shipTable.Contains(x.Id));

					count = Math.Max(
						count,
						y + (y < 5 ? 0 : (fleet.Value.State.Speed == FleetSpeed.Fast ? 1 : 0))
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
			return count >= max_count ? "완료" : "후루타카,카코,아오바,쵸카이,텐류와 고속 1척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
