using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ShipTypeViewModel : ViewModel
	{
		public Action SelectionChangedAction { get; set; }
		public ShipType ShipType { get; private set; }

		#region IsSelected 変更通知プロパティ

		private bool _IsSelected;

		public bool IsSelected
		{
			get { return this._IsSelected; }
			set
			{
				if (this._IsSelected != value)
				{
					this._IsSelected = value;
					this.RaisePropertyChanged();
					if (this.SelectionChangedAction != null) this.SelectionChangedAction();
				}
			}
		}

		#endregion

		public ShipTypeViewModel(ShipType stype)
		{
			this.ShipType = stype;
		}

		public void Set(bool selected)
		{
			this._IsSelected = selected;
			this.RaisePropertyChanged("IsSelected");
		}
	}
}
