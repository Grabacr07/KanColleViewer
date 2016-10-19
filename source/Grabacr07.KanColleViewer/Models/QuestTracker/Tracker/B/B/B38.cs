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
	/// 나치전대 발묘하라!
	/// </summary>
	internal class B38 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 271;
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

				var flagshipTable = new int[]
				{
					63,  // 那智
					192, // 那智改二
					266, // 那智改
				};
				var shipTable = new int[]
				{
					15,  // 曙
					16,  // 潮
					41,  // 初霜
					49,  // 霞
					63,  // 那智
					192, // 那智改二
					231, // 曙改
					233, // 潮改
					241, // 初霜改
					253, // 霞改
					266, // 那智改
					407, // 潮改二
					419, // 初霜改二
					464, // 霞改二
					470, // 霞改二乙
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if (!flagshipTable.Contains(ships[0].Info.Id)) return; // 나치 기함
				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 5) return;

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
			return count >= max_count ? "완료" : "나치 기함,하츠시모,카스미,우시오,아케보노 포함 편성 2-2 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
