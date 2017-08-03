using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 아호작전
	/// </summary>
	internal class Bw1 : NoOverUnderTracker, ITracker
	{
		private int progress_boss;
		private int progress_boss_win;
		private int progress_combat;
		private int progress_combat_s;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 214;
		public QuestType Type => QuestType.Weekly;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				// 출격
				if (args.IsFirstCombat)
					progress_combat = progress_combat.Add(1).Max(36);

				// S 승리
				if (args.Rank == "S")
					progress_combat_s = progress_combat_s.Add(1).Max(6);

				// 보스전
				if (args.IsBoss)
				{
					progress_boss = progress_boss.Add(1).Max(24);

					// 보스전 승리
					if ("SAB".Contains(args.Rank))
						progress_boss_win = progress_boss_win.Add(1).Max(12);
				}

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			progress_combat = 0;
			progress_combat_s = 0;
			progress_boss = 0;
			progress_boss_win = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (progress_combat + progress_combat_s + progress_boss + progress_boss_win) * 100 / (36 + 6 + 24 + 12);
		}

		public string GetProgressText()
		{
			if (progress_combat >= 36 && progress_combat_s >= 6 && progress_boss >= 24 && progress_boss_win >= 12)
				return "완료";

			return "출격 " + progress_combat.ToString() + "/36, S승리 " + progress_combat_s.ToString() + "/6," +
				" 보스전 " + progress_boss.ToString() + "/24, 보스전 승리 " + progress_boss_win.ToString() + "/12";
		}

		public string SerializeData()
		{
			return progress_combat.ToString() + "," + progress_combat_s.ToString() + ","
				+ progress_boss.ToString() + "," + progress_boss_win.ToString();
		}

		public void DeserializeData(string data)
		{
			var part = data.Split(',');
			progress_combat = progress_combat_s = progress_boss = progress_boss_win = 0;

			if (part.Length != 4) return;

			int.TryParse(part[0], out progress_combat);
			int.TryParse(part[1], out progress_combat_s);
			int.TryParse(part[2], out progress_boss);
			int.TryParse(part[3], out progress_boss_win);
		}
	}
}
