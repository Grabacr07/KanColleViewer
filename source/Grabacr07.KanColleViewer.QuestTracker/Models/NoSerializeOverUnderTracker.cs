using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.QuestTracker.Models
{
	internal partial class NoSerializeOverUnderTracker
	{
		public string SerializeData()
		{
			return "0";
		}
		public void DeserializeData(string data)
		{
		}
		public void CheckOverUnder(QuestProgress progress)
		{
			// Do nothing
		}
	}
}
