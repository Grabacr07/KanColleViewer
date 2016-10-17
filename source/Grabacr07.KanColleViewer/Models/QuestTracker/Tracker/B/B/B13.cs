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
	/// 제4전대 출격하라!
	/// </summary>
	internal class B13 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 223;
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
				if (!"SAB".Contains(args.Rank)) return; // 승리

				var shipTable = new int[]
				{
					66,  // 高雄
					67,  // 愛宕
					68,  // 摩耶
					69,  // 鳥海
					269, // 高雄改
					270, // 愛宕改
					271, // 摩耶改
					272, // 鳥海改
					428, // 摩耶改二
					427, // 鳥海改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				if (fleet.Ships.Count(x => shipTable.Contains(x.Info.Id)) < 4) return;

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
			return count >= max_count ? "완료" : "아타고,타카오,쵸카이,마야 포함 편성 2-2 보스전 승리 " + count.ToString() + " / " + max_count.ToString();
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
