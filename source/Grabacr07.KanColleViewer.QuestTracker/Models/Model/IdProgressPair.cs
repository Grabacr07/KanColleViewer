using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Model
{
	public class IdProgressPair
	{
		public int Id { get; set; }
		public QuestProgress Progress { get; set; }
	}
}
