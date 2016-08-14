using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleViewer.Models.QuestTracker.Model;
using Grabacr07.KanColleViewer.Models.QuestTracker.Raw;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.EventArgs
{
    internal class PracticeResultEventArgs
    {
        public bool IsSuccess { get; set; }

        public PracticeResultEventArgs(kcsapi_practice_result data)
        {
            IsSuccess = "SAB".Contains(data.api_win_rank);
        }
    }
}
