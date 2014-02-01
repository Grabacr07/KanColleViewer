using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ExpeditionsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Properties.Resources.Expedition; }
			protected set { throw new NotImplementedException(); }
		}

		public FleetsViewModel Fleets { get; private set; }

		public ExpeditionsViewModel(FleetsViewModel fleets)
		{
			this.Fleets = fleets;
		}
	}
}
