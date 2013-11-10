using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Fleets
{
	public class RepairingBarViewModel : ViewModel
	{
		#region Message 変更通知プロパティ

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public RepairingBarViewModel()
		{
			this.Message = "艦隊に入渠中の艦娘がいます。";
		}	
	}
}
