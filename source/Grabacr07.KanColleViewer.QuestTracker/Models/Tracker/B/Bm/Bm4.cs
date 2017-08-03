﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 수상타격부대 남방으로!
	/// </summary>
	internal class Bm4 : ITracker
	{
		private QuestProgressType lastProgress = QuestProgressType.None;
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 259;
		public QuestType Type => QuestType.Monthly;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var shipList = new int[]
				{
					131, // 大和
					136, // 大和改
					143, // 武蔵
					148, // 武蔵改
					80,  // 長門
					275, // 長門改
					81,  // 陸奥
					276, // 陸奥改
					26,  // 扶桑
					286, // 扶桑改
					411, // 扶桑改二
					27,  // 山城
					287, // 山城改
					412, // 山城改二
					77,  // 伊勢
					82,  // 伊勢改
					87,  // 日向
					88,  // 日向改
				};

				if (args.MapWorldId != 5 || args.MapAreaId != 1) return; // 5-1
				if ("敵前線司令艦隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

				if (!fleet?.Ships.Any(x => x.Info.ShipType.Id == 3) ?? false) return; // 경순양함 없음
				if (fleet?.Ships.Count(x => shipList.Contains(x.Info.Id)) < 3) return; // 야마토급, 나가토급, 후소급, 이세급 전함 합계 3척 미만

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
			return count >= max_count ? "완료" : "전함3척,경순1척 포함 함대로 5-1 보스전 S 승리 " + count.ToString() + " / " + max_count.ToString();
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

		public void CheckOverUnder(QuestProgressType progress)
		{
			if (lastProgress == progress) return;
			lastProgress = progress;

			int cut50 = (int)Math.Ceiling(max_count * 0.5);
			int cut80 = (int)Math.Ceiling(max_count * 0.8);

			switch (progress)
			{
				case QuestProgressType.None:
					if (count >= cut50) count = cut50 - 1;
					break;
				case QuestProgressType.Progress50:
					if (count >= cut80) count = cut80 - 1;
					else if (count < cut50) count = cut50;
					break;
				case QuestProgressType.Progress80:
					if (count < cut80) count = cut80;
					break;
				case QuestProgressType.Complete:
					count = max_count;
					break;
			}
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}
	}
}
