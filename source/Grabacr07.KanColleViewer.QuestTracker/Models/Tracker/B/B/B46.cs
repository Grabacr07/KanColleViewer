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
	/// 제1수뢰전대 케호작전, 돌입하라!
	/// </summary>
	internal class B46 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 278;
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
				if (!"SA".Contains(args.Rank)) return; // A승 이상

				var flagshipTable = new int[]
				{
					114, // 阿武隈
					290, // 阿武隈改
					200, // 阿武隈改二
				};
				var shipTable = new int[]
				{
					114, // 阿武隈
					290, // 阿武隈改
					200, // 阿武隈改二
					35,  // 響
					40,  // 若葉
					41,  // 初霜
					46,  // 五月雨
					50,  // 島風
					147, // Верный
					229, // 島風改
					235, // 響改
					240, // 若葉改
					241, // 初霜改
					246, // 五月雨改
					419, // 初霜改二
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if (!flagshipTable.Contains((ships[0]?.Info.Id ?? 0))) return; // 아부쿠마 기함
				if (fleet?.Ships.Count(x => shipTable.Contains(x.Info.Id)) < 6) return;

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
			return count >= max_count ? "완료" : "아부쿠마 기함,히비키,하츠시모,와카바,사미다레,시마카제 편성 3-2 보스전 A승리 이상 " + count.ToString() + " / " + max_count.ToString();
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
