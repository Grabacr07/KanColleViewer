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
using Grabacr07.KanColleViewer.Models;
using System.Windows;
using Grabacr07.KanColleViewer.Models.Settings;
using System.Windows.Controls;

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
		public KanColleWindowSettings Settings { get; }

		#region Vertical Visibility
		private Visibility _Vertical;
		public Visibility Vertical
		{
			get { return this._Vertical; }
			set
			{
				if (value == this._Vertical) return;
				this._Vertical = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Horizontal VIsibility
		private Visibility _Horizontal;
		public Visibility Horizontal
		{
			get { return this._Horizontal; }
			set
			{
				if (value == this._Horizontal) return;
				this._Horizontal = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region AkashiTimer 변경 통지 프로퍼티

        private AkashiTimerViewModel _AkashiTimer;

        public AkashiTimerViewModel AkashiTimer
        {
            get { return this._AkashiTimer; }
            set
            {
                if (this._AkashiTimer != value)
                {
                    this._AkashiTimer = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region MaterialExtended 변경통지 프로퍼티

        private bool _MaterialExtended;

        public bool MaterialExtended {
            get { return this._MaterialExtended; }
            set
            {
                this._MaterialExtended = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        public InformationViewModel()
		{
			this.Settings = SettingsHost.Instance<KanColleWindowSettings>();
			if (this.Settings?.Dock == Dock.Right || this.Settings?.Dock == Dock.Left || this.Settings?.IsSplit)
			{
				this.Vertical = Visibility.Collapsed;
				this.Horizontal = Visibility.Visible;
			}
			else
			{
				this.Vertical = Visibility.Visible;
				this.Horizontal = Visibility.Collapsed;
			}
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

			_AkashiTimer = new AkashiTimerViewModel();

            KanColleSettings.DisplayMaterialExtended.ValueChanged += (s, e) => this.MaterialExtended = e.NewValue;
            this.MaterialExtended = KanColleSettings.DisplayMaterialExtended;
        }
	}
}
