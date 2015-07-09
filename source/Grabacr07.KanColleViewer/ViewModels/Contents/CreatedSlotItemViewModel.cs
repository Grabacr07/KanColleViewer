using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class CreatedSlotItemViewModel : ViewModel
	{
		#region Succeed 変更通知プロパティ

		private bool? _Succeed;

		public bool? Succeed
		{
			get { return this._Succeed; }
			set
			{
				if (this._Succeed != value)
				{
					this._Succeed = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public CreatedSlotItemViewModel()
		{
			this.Succeed = null;
			this.Name = "-----";
		}

		public void Update(CreatedSlotItem item)
		{
			this.Succeed = item.Succeed;
			this.Name = item.SlotItemInfo.Name;
		}
	}
}
