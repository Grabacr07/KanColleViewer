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
		public bool IsAirplane { get; private set; }

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

			var x = this.Item.Info.Type;
			this.IsAirplane = x == SlotItemType.艦上戦闘機
				|| x == SlotItemType.艦上爆撃機
				|| x == SlotItemType.艦上攻撃機
				|| x == SlotItemType.艦上偵察機
				|| x == SlotItemType.水上偵察機
				|| x == SlotItemType.水上爆撃機
				|| x == SlotItemType.オートジャイロ
				|| x == SlotItemType.対潜哨戒機
				|| x == SlotItemType.大型飛行艇
				|| x == SlotItemType.水上戦闘機
				|| x == SlotItemType.陸上攻撃機
				|| x == SlotItemType.局地戦闘機
				|| x == SlotItemType.艦上偵察機_II;
		}
	}
}
