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
    /// 적 잠수함 제압
    /// </summary>
    internal class Bd8 : ITracker
    {
        private readonly int max_count = 6;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 230;
        public QuestType Type => QuestType.Daily;
        public bool IsTracking { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            manager.BattleResultEvent += (sender, args) =>
            {
                if (!IsTracking) return;

                // 13 > 잠수함
                // 14 > 잠수항모
                count = count.Add(
                        args.EnemyShips
                            .Where(x => x.Type == 13 || x.Type == 14)
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
            return count >= max_count ? "완료" : $"잠수함 격침 {count} / {max_count}";
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
