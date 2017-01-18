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
	/// 미카와 함대를 새로 편성, 진입 준비하라!
	/// </summary>
	internal class A50 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 6;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 152;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

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

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if ((ships[0]?.Info.Id ?? 0) != 427) continue; // 鳥海改二

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Info.Id))
					);
				}

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
			return count >= max_count ? "완료" : "쵸카이改2 기함,후루타카,카코,아오바,텐류,키누가사,유바리 중 5척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
