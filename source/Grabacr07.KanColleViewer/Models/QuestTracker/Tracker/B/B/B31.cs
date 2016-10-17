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
	/// 제2전대 발묘!
	/// </summary>
	internal class B31 : ITracker
	{
		private readonly int max_count = 2;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 258;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 4 && args.MapAreaId != 2) return; // 4-2
				if (args.EnemyName != "東方主力艦隊") return; // boss
				if ("S" != args.Rank) return; // S승리

				var shipTable = new int[]
				{
					26,  // 扶桑
					27,  // 山城
					80,  // 長門
					81,  // 陸奥
					275, // 長門改
					276, // 陸奥改
					286, // 扶桑改
					287, // 山城改
					411, // 扶桑改二
					412, // 山城改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (!shipTable.Contains(ships[0].Info.Id)) return; // 나가토, 무츠, 후소, 야마시로 기함
				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 4) return; // 미포함

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
			return count >= max_count ? "완료" : "나가토,무츠,후소,야마시로 포함 편성, 이중 한 척은 기함으로 하여 4-2 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
