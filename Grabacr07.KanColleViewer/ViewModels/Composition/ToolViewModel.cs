using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class ToolViewModel : ViewModel
	{
		private readonly ITool tool;

		public ToolViewModel(ITool tool)
		{
			this.tool = tool;
		}

		public string Name
		{
			get { return this.tool.Name; }
		}

		public object View
		{
			get { return this.tool.View; }
		}
	}
}
