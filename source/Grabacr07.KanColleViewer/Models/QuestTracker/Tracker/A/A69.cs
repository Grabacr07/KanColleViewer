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
	/// 신편 수뢰전대를 포함하는 함대를 재편성하라!
	/// </summary>
	internal class A69 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 174;
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
					242, // 白露改
					43,  // 時雨
					405, // 春雨
					46,  // 五月雨
					243, // 時雨改
					323, // 春雨改
					246, // 五月雨改
					145, // 時雨改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					var flagship = ships[0].Info.ShipType.Id;
					if (flagship != 3 && flagship != 4 && flagship != 21) continue; // 경순, 중뇌순, 연순

					count = Math.Max(
						count,
						ships.Count(x => x.Info.ShipType.Id == 3 || x.Info.ShipType.Id == 4 || x.Info.ShipType.Id == 21
							|| x.Info.ShipType.Id == 2).Max(4)
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
			return count >= max_count ? "완료" : "경순/중뇌순/연순 기함, 구축함 합계 4척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
