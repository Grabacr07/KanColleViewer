using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleViewer.Views.Catalogs;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class OverviewViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.IntegratedView; }
			protected set { throw new NotImplementedException(); }
		}

		public InformationViewModel Content { get; }


		public OverviewViewModel(InformationViewModel owner)
		{
			this.Content = owner;
		}


		public void Jump(string tabName)
		{
			TabItemViewModel target = null;

			switch (tabName)
			{
				case "Fleets":
					target = this.Content.Fleets;
					break;
				case "Expeditions":
					target = this.Content.Expeditions;
					break;
				case "Quests":
					target = this.Content.Quests;
					break;
				case "Repairyard":
					target = this.Content.Shipyard;
					break;
				case "Dockyard":
					target = this.Content.Shipyard;
					break;
			}

			if (target != null) target.IsSelected = true;
		}

		public void ShowShipCatalog()
		{
			var catalog = new ShipCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(ShipCatalogWindow));
		}

		public void ShowSlotItemCatalog()
		{
			var catalog = new SlotItemCatalogViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(SlotItemCatalogWindow));
		}
		public void CalExp()
		{
			var catalog = new CalExpViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(CalExp));
		}
		public void ExpeditionsCatalogWindow()
		{
			var catalog = new ExpeditionsCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(ExpeditionsCatalogWindow));
		}
		public void ShowNotePad()
		{
			var catalog = new NotePadViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(NotePad));
		}
		public void ShowNdockShipCatalog()
		{
			var catalog = new NeedNdockShipCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(NeedNdockShipCatalogWindow));
		}
		public void ShowRemodelWindow()
		{
			var catalog = new RemodelListWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(RemodelListWindow));
		}
	}
}
