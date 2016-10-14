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
    /// 적 공모 3척 격침
    /// </summary>
    internal class Bd4 : ITracker
    {
        private readonly int max_count = 3;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 211;
        public QuestType Type => QuestType.Daily;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                //  7 > 경공모
                // 11 > 정규공모
                // 18 > 장갑항모
                count = count.Add(
                        args.EnemyShips
                            .Where(x => x.Type == 7 || x.Type == 11 || x.Type == 18)
                            .Where(x => x.MaxHp != int.MaxValue && x.NowHp <= 0)
                            .Count()
                    ).Max(max_count);

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
            return count >= max_count ? "완료" : $"정규공모/경공모 격침 {count} / {max_count}";
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
