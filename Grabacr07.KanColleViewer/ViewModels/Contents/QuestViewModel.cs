using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestViewModel : ViewModel
	{
		public Quest Quest { get; private set; }

		public QuestViewModel(Quest quest)
		{
			this.Quest = quest;
		}
	}

	public class UntakenQuestViewModel : QuestViewModel
	{
		public UntakenQuestViewModel() : base(null) { }
	}
}
