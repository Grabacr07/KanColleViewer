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
	/// 제30구축대(제1차) 출격하라!
	/// </summary>
	internal class B22 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 244;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 3 || args.MapAreaId != 2) return; // 3-2
				if (args.EnemyName != "敵キス島包囲艦隊") return; // boss
				if (!"SABC".Contains(args.Rank)) return; // C패배 이상

				var shipTable = new int[]
				{
					1,   // 睦月
					2,   // 如月
					31,  // 望月
					164, // 弥生
					254, // 睦月改
					255, // 如月改
					261, // 望月改
					308, // 弥生改
					434, // 睦月改二
					435, // 如月改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				if (fleet?.Ships.Count(x => shipTable.Contains(x.Info.Id)) < 4) return;

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
			return count >= max_count ? "완료" : "무츠키,키사라기,야요이,모치즈키 포함 편성 3-2 보스전 C패배 이상 " + count.ToString() + " / " + max_count.ToString();
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
