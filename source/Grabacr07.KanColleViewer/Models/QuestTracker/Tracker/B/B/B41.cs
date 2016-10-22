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
	/// 신 편성 미카와 함대 솔로몬 방면으로!
	/// </summary>
	internal class B41 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 273;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 5 || args.MapAreaId != 1) return; // 5-1
				if (args.EnemyName != "敵前線司令艦隊") return; // boss
				if ("S" != args.Rank) return; // S승리

				var shipTable = new int[]
				{
					427, // 鳥海改二
					59,  // 古鷹
					60,  // 加古
					61,  // 青葉
					51,  // 天龍
					123, // 衣笠
					115, // 夕張
					262, // 古鷹改
					263, // 加古改
					264, // 青葉改
					213, // 天龍改
					295, // 衣笠改
					293, // 夕張改
					416, // 古鷹改二
					417, // 加古改二
					142, // 衣笠改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if ((ships[0]?.Info.Id ?? 0) != 427) return; // 쵸카이改2 기함
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
			return count >= max_count ? "완료" : "쵸카이改2 기함,후루타카,카코,아오바,유바리,텐류 편성 5-1 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
