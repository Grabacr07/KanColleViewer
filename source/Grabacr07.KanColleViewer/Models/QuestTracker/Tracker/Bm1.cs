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
    /// 제5전대 출격하라!
    /// </summary>
    internal class Bm1 : ITracker
    {
        private readonly int max_count = 1;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 249;
        public QuestType Type => QuestType.Monthly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "敵主力艦隊" // 2-1, 2-5
            };

            var shipList1 = new int[]
            {
                62,  // 妙高
                265, // 妙高改
                319, // 妙高改二
            };
            var shipList2 = new int[]
            {
                63,  // 那智
                266, // 那智改
                192, // 那智改二
            };
            var shipList3 = new int[]
            {
                65,  // 羽黒
                268, // 羽黒改
                194, // 羽黒改二
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 2) return; // 2 해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (args.Rank != "S") return;

                if (!args.EnemyShips.Any(x => x.Id == 542 || x.Id == 543)) return;
                // 2-5 보스는 전함타급 엘리트 혹은 플래그십이 등장
                // 542, 543은 각각 타급 엘리트, 플래그십 id

                var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
                if (
                    !fleet.Ships.Any(x => shipList1.Contains(x.Info?.Id ?? 0)) ||
                    !fleet.Ships.Any(x => shipList2.Contains(x.Info?.Id ?? 0)) ||
                    !fleet.Ships.Any(x => shipList3.Contains(x.Info?.Id ?? 0))
                ) return; // 묘코, 나치, 하구로 없음

                count = count.Add(1).Max(max_count);

                ProcessChanged?.Invoke(this, emptyEventArgs);
            };
        }

        public void ResetQuest()
        {
            count = 0;
            ProcessChanged?.Invoke(this, emptyEventArgs);
        }

        public double GetProgress()
        {
            return (double)count / max_count * 100;
        }

        public string GetProgressText()
        {
            return count >= max_count ? "완료" : $"묘코,나치,하구로 포함 함대로 2-5 보스전 S 승리 {count} / {max_count}";
        }

        public string SerializeData()
        {
            return $"{count}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                count = int.Parse(data);
            }
            catch
            {
                count = 0;
            }
        }
    }
}
