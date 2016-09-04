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
    /// 적 북방함대 주력 격멸
    /// </summary>
    internal class Bw7 : ITracker
    {
        private readonly int max_count = 5;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 241;
        public QuestType Type => QuestType.Weekly;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            var BossNameList = new string[]
            {
                "深海棲艦泊地艦隊",     // 3-3
                "深海棲艦北方艦隊中枢", // 3-4
                "北方増援部隊主力"      // 3-5
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 3) return; // 3 해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (!"SAB".Contains(args.Rank)) return;

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
            return count >= max_count ? "완료" : $"3-3 ~ 3-5 보스전 승리 {count} / {max_count}";
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
