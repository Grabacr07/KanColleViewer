using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker;

namespace Grabacr07.KanColleViewer.QuestTracker.Models
{
	public interface ITracker
	{
		int Id { get; }
		bool IsTracking { get; set; }
		QuestType Type { get; }

		void RegisterEvent(TrackManager manager);
		void ResetQuest();

		event EventHandler ProcessChanged;

		string GetProgressText();
		int GetProgress();

		string SerializeData();
		void DeserializeData(string data);
	}
}
