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
    /// 항모기동부대 서쪽으로!
    /// </summary>
    internal class Bm6 : ITracker
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
                "東方主力艦隊" // 4-2, 4-3
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 5) return; // 5 해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (args.Rank != "S") return;

                if (!args.EnemyShips.Any(x => x.Id == 529)) return;
                // 4-2 보스는 전함루급 플래그십이 등장, 529은 루급 플래그십 id

                var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

                //  2 > 구축함
                //  7 > 경공모
                // 11 > 정규공모
                // 18 > 장갑공모
                if (fleet.Ships.Count(x => x.Info.ShipType.Id == 2) < 2) return; // 구축함 2척 미만
                if (fleet.Ships.Count(x => new int[] { 7, 11, 18 }.Contains(x.Info.ShipType.Id)) < 2) return; // 공모 2척 미만

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
            return count >= max_count ? "완료" : $"구축2,공모2 포함 함대로 4-2 보스전 S 승리 {count} / {max_count}";
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
