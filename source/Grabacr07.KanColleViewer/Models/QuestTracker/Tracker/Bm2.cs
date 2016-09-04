using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Tracker
{
    /// <summary>
    /// 잠수함대 출격하라!
    /// </summary>
    internal class Bm2 : ITracker
    {
        private readonly int max_count = 3;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 256;
        public QuestType Type => QuestType.Monthly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "敵回航中空母" // 6-1
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 6) return; // 6 해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (args.Rank != "S") return;

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
            return count >= max_count ? "완료" : $"6-1 보스전 S 승리 {count} / {max_count}";
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
