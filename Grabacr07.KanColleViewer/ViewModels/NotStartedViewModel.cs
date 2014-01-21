using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class NotStartedViewModel : ViewModel
	{
		private static readonly NotStartedViewModel instance = new NotStartedViewModel();

		public static NotStartedViewModel Instance
		{
			get { return instance; }
		}

		private NotStartedViewModel() { }
	}
}
