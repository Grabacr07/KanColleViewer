using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Livet.Messaging;
using System.Windows;
using Grabacr07.KanColleViewer.Models;
using Settings2 = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class OverviewViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.IntegratedView; }
			protected set { throw new NotImplementedException(); }
		}
		#region VerticalBar 変更通知プロパティ

		private Visibility _VerticalBar;

		public Visibility VerticalBar
		{
			get { return this._VerticalBar; }
			set
			{
				if (this._VerticalBar != value)
				{
					this._VerticalBar = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region HorizontalBar 変更通知プロパティ

		private Visibility _HorizontalBar;

		public Visibility HorizontalBar
		{
			get { return this._HorizontalBar; }
			set
			{
				if (this._HorizontalBar != value)
				{
					this._HorizontalBar = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ThirdColumn 変更通知プロパティ

		private double _ThirdColumn;

		public double ThirdColumn
		{
			get { return this._ThirdColumn; }
			set
			{
				if (this._ThirdColumn != value)
				{
					this._ThirdColumn = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public MainContentViewModel Content { get; private set; }


		public OverviewViewModel(MainContentViewModel owner)
		{
			if (Settings2.Current.Orientation == OrientationType.Vertical)
			{
				this.VerticalBar = Visibility.Visible;
				this.HorizontalBar = Visibility.Collapsed;
				this.ThirdColumn = 205.0;
			}
			else
			{
				this.VerticalBar = Visibility.Collapsed;
				this.HorizontalBar = Visibility.Visible;
				this.ThirdColumn = 230.0;
			}

			Settings2.Current.VerticalWindow += () =>
			{
				this.VerticalBar = Visibility.Visible;
				this.HorizontalBar = Visibility.Collapsed;
				this.ThirdColumn = 205.0;
			};
			Settings2.Current.HorizontalWindow += () =>
			{
				this.VerticalBar = Visibility.Collapsed;
				this.HorizontalBar = Visibility.Visible;
				this.ThirdColumn = 230.0;
			};
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
			var message = new TransitionMessage(catalog, "Show/ShipCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}

		public void ShowSlotItemCatalog()
		{
			var catalog = new SlotItemCatalogViewModel();
			var message = new TransitionMessage(catalog, "Show/SlotItemCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}
		public void CalExp()
		{
			var catalog = new CalExpViewModel();
			var message = new TransitionMessage(catalog, "Show/CalExp");
			this.Messenger.RaiseAsync(message);
		}
		public void ExpeditionsCatalogWindow()
		{
			var catalog = new ExpeditionsCatalogWindowViewModel();
			var message = new TransitionMessage(catalog, "Show/ExpeditionsCatalogWindow");
			this.Messenger.RaiseAsync(message);
		}
		public void ShowNotePad()
		{
			var window = new NotePadViewModel();
			var message = new TransitionMessage(window, "Show/NotePad");
			this.Messenger.RaiseAsync(message);
		}
	}
}
