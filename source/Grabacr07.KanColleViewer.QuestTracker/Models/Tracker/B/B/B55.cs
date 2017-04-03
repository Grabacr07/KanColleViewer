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
	/// 제16전대(제2차) 출격하라!
	/// </summary>
	internal class B55 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 295;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 3) return; // 2-3
				if (args.EnemyName != "敵主力打撃群") return; // boss
				if ("S" != args.Rank) return; // S승리

				var flagshipTable = new int[]
				{
					53,  // 名取
					221, // 名取改
				};
				var shipTable = new int[]
				{
					22,  // 五十鈴
					53,  // 名取
					113, // 鬼怒
					123, // 衣笠
					141, // 五十鈴改二
					142, // 衣笠改二
					219, // 五十鈴改
					221, // 名取改
					289, // 鬼怒改
					295, // 衣笠改
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if (!flagshipTable.Contains((ships[0]?.Info.Id ?? 0))) return; // 나토리 기함
				if (ships.Count(x => shipTable.Contains(x.Info.Id)) < 3) return;

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
			return count >= max_count ? "완료" : "나토리 기함,이스즈,키누 포함 편성 2-3 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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

		public void CheckOverUnder(QuestProgress progress)
		{
			int cut50 = (int)Math.Ceiling(max_count * 0.5);
			int cut80 = (int)Math.Ceiling(max_count * 0.8);

			switch (progress)
			{
				case QuestProgress.None:
					if (count >= cut50) count = cut50 - 1;
					break;
				case QuestProgress.Progress50:
					if (count >= cut80) count = cut80 - 1;
					else if (count < cut50) count = cut50;
					break;
				case QuestProgress.Progress80:
					if (count < cut80) count = cut80;
					break;
			}
		}
	}
}
