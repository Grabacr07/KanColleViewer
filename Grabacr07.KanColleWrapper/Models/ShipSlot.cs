using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ShipSlot : NotificationObject
	{
		public SlotItem Item { get; private set; }

		public int Maximum { get; private set; }

		public bool Equipped
		{
			get { return this.Item != null; }
		}

		#region Current 変更通知プロパティ

		private int _Current;

		public int Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ShipSlot(SlotItem item, int maximum, int current)
		{
			this.Item = item;
			this.Maximum = maximum;
			this.Current = current;
		}
	}
}