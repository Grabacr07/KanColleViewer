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
	/// 제1수뢰전대 케호작전, 재돌입!
	/// </summary>
	internal class B47 : ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 279;
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
				if ("S" == args.Rank) return; // A승 이상

				var shipTable = new int[]
				{
					200, // 阿武隈改二
					35,  // 響
					50,  // 島風
					132, // 秋雲
					133, // 夕雲
					135, // 長波
					147, // Верный
					229, // 島風改
					235, // 響改
					301, // 秋雲改
					302, // 夕雲改
					304, // 長波改
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				var ships = fleet?.Ships;

				if (ships[0].Info.Id != 200) return; // 아부쿠마改2 기함
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
			return count >= max_count ? "완료" : "아부쿠마改2 기함,히비키,유구모,나가나미,아키구모,시마카제 편성 3-2 보스전 S승리 " + count.ToString() + " / " + max_count.ToString();
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
