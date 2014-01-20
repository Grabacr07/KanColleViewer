using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public abstract class TimerViewModel : ViewModel
	{
		public Func<string> TimerProc
		{
			get { return this.CreateMessage; }
		}

		protected abstract string CreateMessage();
	}
}
