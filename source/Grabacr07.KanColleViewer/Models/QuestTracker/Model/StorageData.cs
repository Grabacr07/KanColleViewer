using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Model
{
    public class StorageData
    {
        public int Id { get; set; }
        public DateTime TrackTime { get; set; }
        public string Serialized { get; set; }
    }
}
