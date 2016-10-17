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
	/// 전함부대 북방해역에 돌입하라!
	/// </summary>
	internal class B32 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 260;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 3 || args.MapAreaId != 5) return; // 3-5
				if (args.EnemyName != "北方増援部隊主力") return; // boss
				if ("S" != args.Rank) return; // S승리

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (ships.Count(x => x.Info.ShipType.Id == 8 || x.Info.ShipType.Id == 9) != 2) return; // 전함 2척
				if (ships.Count(x => x.Info.ShipType.Id == 7) != 1) return; // 경공모 1척

				count = count.Add(1).Max(max_count);

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
			return count >= max_count ? "완료" : "전함 2척,경공모 1척 포함 편성 3-5 보스전 S승리 " + count.ToString() + " / " + max_count.ToString() + " (초과 안됨,정규/장갑공모 안됨)";
		}

		public string SerializeData()
		{
			return count.ToString();
		}

		public void DeserializeData(string data)
		{
			count = 0;
			int.TryParse(data, out count);
		}
	}
}
