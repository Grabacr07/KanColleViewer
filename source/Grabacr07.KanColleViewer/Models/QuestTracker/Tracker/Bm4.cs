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
    /// 수상타격부대 남방으로!
    /// </summary>
    internal class Bm4 : ITracker
    {
        private readonly int max_count = 1;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 259;
        public QuestType Type => QuestType.Monthly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "敵前線司令艦隊" // 1-4
            };
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
                27,  // 山城
                287, // 山城改
                77,  // 伊勢
                82,  // 伊勢改
                87,  // 日向
                88,  // 日向改
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 5) return; // 5 해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (args.Rank != "S") return;

                var fleet = KanColleClient.Current.Homeport.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

                // 3 > 경순양함
                if (!fleet.Ships.Any(x => x.Info.ShipType.Id == 3)) return; // 경순양함 없음
                if (fleet.Ships.Count(x => shipList.Contains(x.Info.Id)) < 3) return; // 야마토급, 나가토급, 후소급, 이세급 전함 합계 3척 미만

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
            return count >= max_count ? "완료" : $"전함3척,경순1척 포함 함대로 5-1 보스전 S 승리 {count} / {max_count}";
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
