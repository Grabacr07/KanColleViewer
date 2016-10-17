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
	/// 제5항공전대 산호열도 앞바다로 출격하라!
	/// </summary>
	internal class B50 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 287;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 5 && args.MapAreaId != 2) return; // 5-2
				if (args.EnemyName != "敵機動部隊本隊") return; // boss
				if ("S" == args.Rank) return; // S승리

				var shipTable = new int[]
				{
					93,  // 朧
					110, // 翔鶴
					111, // 瑞鶴
					112, // 瑞鶴改
					132, // 秋雲
					230, // 朧改
					288, // 翔鶴改
					301, // 秋雲改
					461, // 翔鶴改二
					462, // 瑞鶴改二
					466, // 翔鶴改二甲
					467, // 瑞鶴改二甲
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 4) return;
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
			return count >= max_count ? "완료" : "쇼카쿠,즈이카쿠,오보로,아키구모 포함 편성 5-2 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
