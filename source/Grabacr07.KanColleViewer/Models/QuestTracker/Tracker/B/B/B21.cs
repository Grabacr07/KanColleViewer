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
	/// 제18구축대 출격하라!
	/// </summary>
	internal class B21 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 240;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 3 && args.MapAreaId != 1) return; // 3-1
				if (args.EnemyName != "敵北方侵攻艦隊") return; // boss
				if (!"SAB".Contains(args.Rank)) return; // 승리

				var shipTable = new int[]
				{
					17,  // 陽炎
					18,  // 不知火
					48,  // 霰
					49,  // 霞
					225, // 陽炎改
					226, // 不知火改
					252, // 霰改
					253, // 霞改
					464, // 霞改二
					470, // 霞改二乙
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
			return count >= max_count ? "완료" : "카스미,아라레,카게로,시라누이 포함 편성 3-1 보스전 승리 " + count.ToString() + " / " + max_count.ToString();
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
