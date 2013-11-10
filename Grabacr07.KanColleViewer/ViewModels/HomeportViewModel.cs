using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Docks;
using Grabacr07.KanColleViewer.ViewModels.Fleets;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class HomeportViewModel : ViewModel
	{
		public AdmiralViewModel Admiral { get; private set; }
		public MaterialsViewModel Materials { get; private set; }
		public FleetsViewModel Fleets { get; private set; }
		public ShipsViewModel Ships { get; private set; }
		public SlotItemsViewModel SlotItems { get; private set; }
		public DockyardViewModel Dockyard { get; private set; }
		public RepairyardViewModel Repairyard { get; private set; }

		public HomeportViewModel()
		{
			this.Admiral = new AdmiralViewModel();
			this.Materials = new MaterialsViewModel();
			this.Fleets = new FleetsViewModel();
			this.Ships = new ShipsViewModel();
			this.SlotItems = new SlotItemsViewModel();
			this.Dockyard = new DockyardViewModel();
			this.Repairyard = new RepairyardViewModel();
		}
	}
}
