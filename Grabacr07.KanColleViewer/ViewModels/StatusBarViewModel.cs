using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class StatusBarViewModel : ViewModel
	{
		#region NotificationMessage 変更通知プロパティ

		private string _NotificationMessage;

		public string NotificationMessage
		{
			get { return this._NotificationMessage; }
			set
			{
				if (this._NotificationMessage != value)
				{
					this._NotificationMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

	}
}
