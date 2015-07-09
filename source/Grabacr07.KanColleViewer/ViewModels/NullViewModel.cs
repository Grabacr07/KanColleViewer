using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public sealed class NullViewModel : ViewModel
	{
		public static NullViewModel Instance { get; } = new NullViewModel();
		
		private NullViewModel() { }
	}
}
