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
	/// 제22구축대 출격하라!
	/// </summary>
	internal class B39 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 270;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 1 || args.MapAreaId != 4) return; // 1-4
				if (args.EnemyName != "敵機動部隊") return; // boss
				if ("S" != args.Rank) return; // S승리

				var shipTable = new int[]
				{
					6,   // 長月
					28,  // 皐月
					29,  // 文月
					256, // 皐月改
					257, // 文月改
					258, // 長月改
					418, // 皐月改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 3) return;
				if (ships.Count(x => x.Info.ShipType.Id == 2) < 4) return; // 구축함 4척

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
			return count >= max_count ? "완료" : "사츠키,후미즈키,나가츠키 포함 편성 1-4 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
