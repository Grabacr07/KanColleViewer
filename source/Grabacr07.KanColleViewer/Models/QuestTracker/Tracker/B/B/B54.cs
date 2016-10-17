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
	/// 오자와 함대 출격하라!
	/// </summary>
	internal class B54 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 294;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 4) return; // 2-4
				if (args.EnemyName != "敵侵攻中核艦隊") return; // boss
				if ("S" == args.Rank) return; // S승리

				var flagshipTable = new int[]
				{
					112, // 瑞鶴改
					462, // 瑞鶴改二
					467, // 瑞鶴改二甲
				};
				var shipTable = new int[]
				{
					112, // 瑞鶴改
					462, // 瑞鶴改二
					467, // 瑞鶴改二甲
					82,  // 伊勢改
					88,  // 日向改
					117, // 瑞鳳改
					108, // 千歳航
					109, // 千代田航
					291, // 千歳航改
					292, // 千代田航改
					296, // 千歳航改二
					297, // 千代田航改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (!flagshipTable.Contains(ships[0].Info.Id)) return; // 즈이카쿠改 기함
				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 6) return;

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
			return count >= max_count ? "완료" : "즈이카쿠改 기함,치토세航,치요다航,즈이호改,이세改,휴가改 편성 2-4 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
