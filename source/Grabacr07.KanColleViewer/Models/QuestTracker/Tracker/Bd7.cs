using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Tracker
{
    /// <summary>
    /// 남서제도해역 제해권 획득
    /// </summary>
    internal class Bd7 : ITracker
    {
        private readonly int max_count = 5;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 226;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            List<string> BossNameList = new List<string>
            {
                "敵主力艦隊",
                "敵通商破壊艦隊",
                "敵主力打撃群",
                "敵侵攻中核艦隊",
                //"敵主力艦隊"
            };

            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                if (args.MapAreaId != 2) return; // 2해역
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
            return count >= max_count ? "완료" : $"2-1 ~ 2-5 보스전 승리 {count} / {max_count}";
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
