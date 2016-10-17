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
	/// 수뢰전대 바시섬 앞바다 긴급전개 출격하라!
	/// </summary>
	internal class B30 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 255;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 2) return; // 2-2
				if (args.EnemyName != "敵通商破壊艦隊") return; // boss
				if ("S" != args.Rank) return; // S승리

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (ships[0].Info.ShipType.Id != 3) return; // 기함 경순양함 이외
				if (ships.Any(x => x.Info.ShipType.Id != 2 && x.Info.ShipType.Id != 3)) return; // 구축함, 경순양함 이외 함종
				if (ships.Count(x => x.Info.ShipType.Id == 3) > 3) return; // 경순양함 3척 이상
				if (ships.Count(x => x.Info.ShipType.Id == 2) < 3) return; // 구축함 3척 미만

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
			return count >= max_count ? "완료" : "경순 기함 최대 2척,구축 3척 이상 편성 2-2 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
