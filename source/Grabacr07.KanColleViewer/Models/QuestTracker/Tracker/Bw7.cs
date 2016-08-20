using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

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
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            List<string> BossNameList = new List<string>
            {
                "深海棲艦泊地艦隊",
                "深海棲艦北方艦隊中枢",
                "北方増援部隊主力"
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 3) return; // 3해역
                if (!BossNameList.Contains(args.EnemyName)) return;
                if (!"SAB".Contains(args.Rank)) return;

                count += count >= max_count ? 0 : 1;
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
