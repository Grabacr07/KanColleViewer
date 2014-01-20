using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class StatusBarItemViewModel : ViewModel
	{
		#region Dock 変更通知プロパティ

		private Dock _Dock;

		public Dock Dock
		{
			get { return this._Dock; }
			set
			{
				if (this._Dock != value)
				{
					this._Dock = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		
	}
}
