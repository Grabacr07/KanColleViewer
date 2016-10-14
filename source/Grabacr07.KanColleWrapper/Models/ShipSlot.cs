using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ShipSlot : Notifier
	{
		public SlotItem Item { get; }

		public int Maximum { get; private set; }
        public int Lost { get; private set; }

		public bool Equipped => this.Item != null && this.Item != SlotItem.Dummy;

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
			this.Item = item ?? SlotItem.Dummy;
			this.Maximum = maximum;
			this.Current = current;
            this.Lost = Maximum - Current;
        }
	}
}
