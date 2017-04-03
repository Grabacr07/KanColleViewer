using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	///  수뢰전대, 남서제도해역을 초계하라!
	/// </summary>
	internal class B77 : NoOverUnderTracker, ITracker
	{
		private readonly int max_count = 2;
		private int count1, count2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 825;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || (args.MapAreaId != 2 && args.MapAreaId != 3)) return; // 2-2 or 2-3
				if ("S" != args.Rank) return; // S승리

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;
				var flagship = ships[0]?.Info.ShipType.Id;

				if (flagship != 3) return; // 기함 경순
				if (ships.Count(x => x.Info.ShipType.Id == 2) < 4) return; // 구축 4척

				if (args.MapAreaId == 2)
				{
					if (args.EnemyName != "敵主力艦隊") return; // boss
					count1 = count1.Add(1).Max(1);
				}
				else if (args.MapAreaId == 3)
				{
					if (args.EnemyName != "敵通商破壊艦隊") return; // boss
					count2 = count2.Add(1).Max(1);
				}

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count1 = count2 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count1 + count2) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count1 + count2) >= max_count ? "완료" : "경순 기함,구축 4척 포함 편성 2-2 보스전 S승리 " + count1.ToString() + "/1, 2-3 보스전 S승리 " + count2.ToString() + "/1";
		}

		public string SerializeData()
		{
			return count1.ToString() + "," + count2.ToString();
		}

		public void DeserializeData(string data)
		{
			count1 = count2 = 0;

			string[] part = data.Split(',');
			if (part.Length != 2) return;

			int.TryParse(part[0], out count1);
			int.TryParse(part[1], out count1);
		}
	}
}
