using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleViewer.ViewModels.Dev;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainContentViewModel : ViewModel
	{
		public AdmiralViewModel Admiral { get; private set; }
		public MaterialsViewModel Materials { get; private set; }
		public ShipsViewModel Ships { get; private set; }
		public SlotItemsViewModel SlotItems { get; private set; }

		public FleetsViewModel Fleets { get; }
		public ShipyardViewModel Shipyard { get; }
		public QuestsViewModel Quests { get; }
		public ExpeditionsViewModel Expeditions { get; }

		public IList<TabItemViewModel> TabItems { get; set; }
		public IList<TabItemViewModel> SystemTabItems { get; set; }

		public VolumeViewModel Volume { get; private set; }

		#region SelectedItem 変更通知プロパティ

		private TabItemViewModel _SelectedItem;

		public TabItemViewModel SelectedItem
		{
			get { return this._SelectedItem; }
			set
			{
				if (this._SelectedItem != value)
				{
					this._SelectedItem = value;
					this.RaisePropertyChanged();

					App.ViewModelRoot.StatusBar = value;
				}
			}
		}

		#endregion


		public MainContentViewModel()
		{
			this.Admiral = new AdmiralViewModel();
			this.Materials = new MaterialsViewModel();
			this.Ships = new ShipsViewModel();
			this.SlotItems = new SlotItemsViewModel();

			this.Fleets = new FleetsViewModel();
			this.Shipyard = new ShipyardViewModel();
			this.Quests = new QuestsViewModel();
			this.Expeditions = new ExpeditionsViewModel(this.Fleets);

			this.TabItems = new List<TabItemViewModel>
			{
				new OverviewViewModel(this),
				this.Fleets,
				this.Shipyard,
				this.Quests,
				this.Expeditions,
				new ToolsViewModel(),
			};
			this.SystemTabItems = new List<TabItemViewModel>
			{
				new SettingsViewModel(),
				#region DEBUG
#if DEBUG
				new DebugTabViewModel(),
#endif
				#endregion
			};
			this.SelectedItem = this.TabItems.FirstOrDefault();

			this.Volume = new VolumeViewModel();
		}
	}
}
