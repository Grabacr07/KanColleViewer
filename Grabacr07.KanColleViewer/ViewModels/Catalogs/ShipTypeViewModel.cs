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

		#region Id 変更通知プロパティ

		private int _Id;

		public int Id
		{
			get { return this._Id; }
			set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region DisplayName 変更通知プロパティ

		private string _DisplayName;

		public string DisplayName
		{
			get { return this._DisplayName; }
			set
			{
				if (this._DisplayName != value)
				{
					this._DisplayName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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
			this.Id = stype.Id;

			// [ID = 8 (金剛型戦艦)] と [ID = 9 (金剛型以外の戦艦)] がどちらも "戦艦" 表記で区別がつかないため、
			// ID = 8 の方を "巡洋戦艦" に変更
			this.DisplayName = (stype.Id == 8 && stype.Name == "戦艦") ? "巡洋戦艦" : stype.Name;
		}

		public void Set(bool selected)
		{
			this._IsSelected = selected;
			this.RaisePropertyChanged("IsSelected");
		}
	}
}
