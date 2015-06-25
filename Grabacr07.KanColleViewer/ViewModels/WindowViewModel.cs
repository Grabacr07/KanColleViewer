using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class WindowViewModel : ViewModel
	{
		#region Title 変更通知プロパティ

		private string _Title = "Window";

		public string Title
		{
			get { return this._Title; }
			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public WindowState WindowState { get; set; }

		public virtual void Initialize() { }
	}
}
