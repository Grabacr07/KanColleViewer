using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Contents.Docks;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainContentViewModel : ViewModel
	{
		public AdmiralViewModel Admiral { get; private set; }
		public MaterialsViewModel Materials { get; private set; }
		public ShipsViewModel Ships { get; private set; }
		public SlotItemsViewModel SlotItems { get; private set; }

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

			var fleets = new FleetsViewModel();

			this.TabItems = new List<TabItemViewModel>
			{
				fleets,
				new RepairyardViewModel(),
				new DockyardViewModel(),
				new QuestsViewModel(),
				new ExpeditionsViewModel(fleets),
			};
			this.SystemTabItems = new List<TabItemViewModel>
			{
				new SettingsViewModel(),
			};
			this.SelectedItem = this.TabItems.FirstOrDefault();

			this.Volume = new VolumeViewModel();
		}
	}
}
