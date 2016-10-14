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
    /// 신 조함 건조 지령
    /// </summary>
    internal class F6 : ITracker
    {
        private readonly int max_count = 1;
        private int count;

        public event EventHandler ProcessChanged;

        int ITracker.Id => 606;
        public QuestType Type => QuestType.Daily;

        private bool _IsTracking;
        public bool IsTracking {
            get { return _IsTracking; }
            set
            {
                if (value) TrackStartTime = TrackManager.TokyoDateTime;
                _IsTracking = value;
            }
        }
        public DateTime TrackStartTime { get; set; }

        private System.EventArgs emptyEventArgs = new System.EventArgs();

        public void RegisterEvent(TrackManager manager)
        {
            manager.CreateShipEvent += (sender, args) =>
            {
                if (!IsTracking) return;

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
            return count >= max_count ? "완료" : $"칸무스 건조 {count} / {max_count}";
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
