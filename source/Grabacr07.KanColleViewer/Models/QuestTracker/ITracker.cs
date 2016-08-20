using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Models.QuestTracker
{
    internal interface ITracker
    {
        int Id { get; }
        bool IsTracking { get; set; }

        void RegisterEvent(TrackManager manager);
        void ResetQuest();

        event EventHandler ProcessChanged;

        string GetProgressText();
        double GetProgress();

        string SerializeData();
        void DeserializeData(string data);
    }
}
