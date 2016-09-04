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
    /// 수상반격부대 돌입하라!
    /// </summary>
    internal class Bm7 : ITracker
    {
        private readonly int max_count = 1;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 264;
        public QuestType Type => QuestType.Monthly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "敵主力艦隊" // 2-1, 2-5
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

                // 2 > 구축함
                // 3 > 경순양함
                // 5 > 중순양함
                if (fleet.Ships[0].Info.ShipType.Id != 2) return; // 기함 구축함 이외
                if (fleet.Ships.Count(x => x.Info.ShipType.Id == 2) != 4) return; // 구축함 4척 이외
                if (fleet.Ships.Count(x => x.Info.ShipType.Id == 3) != 1) return; // 경순양함 1척 이외
                if (fleet.Ships.Count(x => x.Info.ShipType.Id == 5) != 1) return; // 중순양함 1척 이외

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
            return count >= max_count ? "완료" : $"구축함 기함,구축4,중순1,경순1 함대로 2-5 보스전 S 승리 {count} / {max_count}";
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
