using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleViewer.ViewModels.Dev;
using Grabacr07.KanColleViewer.ViewModels.Settings;
using Livet;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class InformationViewModel : ViewModel
	{
		// ----- Tab items

		public OverviewViewModel Overview { get; }
		public FleetsViewModel Fleets { get; }
		public ShipyardViewModel Shipyard { get; }
		public QuestsViewModel Quests { get; }
		public ExpeditionsViewModel Expeditions { get; }
		public ToolsViewModel Tools { get; }

		public IList<TabItemViewModel> TabItems { get; set; }
		public IList<TabItemViewModel> SystemTabItems { get; set; }

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
				}
			}
		}

		#endregion


		// ----- Other elements

		public AdmiralViewModel Admiral { get; }
		public MaterialsViewModel Materials { get; }
		public ShipsViewModel Ships { get; }
		public SlotItemsViewModel SlotItems { get; }


		public InformationViewModel()
		{
			this.TabItems = new List<TabItemViewModel>
			{
				(this.Overview = new OverviewViewModel(this).AddTo(this)),
				(this.Fleets = new FleetsViewModel().AddTo(this)),
				(this.Shipyard = new ShipyardViewModel().AddTo(this)),
				(this.Quests = new QuestsViewModel().AddTo(this)),
				(this.Expeditions = new ExpeditionsViewModel(this.Fleets).AddTo(this)),
				(this.Tools = new ToolsViewModel().AddTo(this)),
			};
			this.SystemTabItems = new List<TabItemViewModel>
			{
				SettingsViewModel.Instance,
				#region DEBUG
#if DEBUG
				new DebugTabViewModel().AddTo(this),
#endif
				#endregion
			};
			this.SelectedItem = this.TabItems.FirstOrDefault();

			this.Admiral = new AdmiralViewModel().AddTo(this);
			this.Materials = new MaterialsViewModel().AddTo(this);
			this.Ships = new ShipsViewModel().AddTo(this);
			this.SlotItems = new SlotItemsViewModel().AddTo(this);
		}
	}
}
