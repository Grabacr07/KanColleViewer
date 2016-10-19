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
	/// 레호작전 실시하라!
	/// </summary>
	internal class B57 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 297;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 5) return; // 2-5
				if (args.EnemyName != "敵主力艦隊") return; // boss
				if ("S" == args.Rank) return; // S승리

				var flagshipTable = new int[]
				{
					49,  // 霞
					253, // 霞改
					464, // 霞改二
					470, // 霞改二乙
				};
				var shipTable = new int[]
				{
					49,  // 霞
					64,  // 足柄
					183, // 大淀
					193, // 足柄改二
					253, // 霞改
					267, // 足柄改
					321, // 大淀改
					325, // 清霜改
					344, // 朝霜改
					410, // 清霜
					425, // 朝霜
					464, // 霞改二
					470, // 霞改二乙
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet.Ships;

				if (!flagshipTable.Contains(ships[0].Info.Id)) return; // 카스미 기함
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
			return count >= max_count ? "완료" : "카스미 기함,아시가라,오요도,아사시모,키요시모 포함 편성 2-5 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
