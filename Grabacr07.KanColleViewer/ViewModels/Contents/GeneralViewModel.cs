using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class GeneralViewModel : TabItemViewModel
	{
		public MainContentViewModel Content { get; private set; }

		public GeneralViewModel(MainContentViewModel owner)
		{
			this.Content = owner;
		}
	}
}
