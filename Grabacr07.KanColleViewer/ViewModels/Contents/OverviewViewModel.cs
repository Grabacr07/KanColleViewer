using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Livet;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class OverviewViewModel : TabItemViewModel
	{
		public MainContentViewModel Content { get; private set; }


		public OverviewViewModel(MainContentViewModel owner)
		{
			this.Name = Resources.IntegratedView;
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
					target = this.Content.Repairyard;
					break;
				case "Dockyard":
					target = this.Content.Dockyard;
					break;
			}

			if (target != null) target.IsSelected = true;
		}

		public void ShowShipCatalog()
		{
			var catalog = new ShipCatalogWindowViewModel();
			var message = new TransitionMessage(catalog, "Show/ShipCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}

		public void ShowSlotItemCatalog()
		{
			var catalog = new SlotItemCatalogViewModel();
			var message = new TransitionMessage(catalog, "Show/SlotItemCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}
	}
}
