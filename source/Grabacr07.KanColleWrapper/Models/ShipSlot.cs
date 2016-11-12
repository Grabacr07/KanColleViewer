using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ShipSlot : Notifier
	{
		public ShipInfo Owner { get; }
		public SlotItem Item { get; }

		public int Maximum { get; private set; }
		public int Lost { get; private set; }
		public bool IsAirplane => this.Item.Info.Type.IsNumerable();

		public int FitValue => this.CalculateFit();
		public string Tooltip => this.Item.Info.Id == 0 ? null : this.Item.NameWithLevel;

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

		public ShipSlot(Ship owner, SlotItem item, int maximum, int current)
		{
			this.Owner = owner.Info ?? ShipInfo.Dummy;
			this.Item = item ?? SlotItem.Dummy;

			this.Maximum = maximum;
			this.Current = current;
			this.Lost = Maximum - Current;
		}

		private int CalculateFit()
		{
			ShipFitClass shipClass = ShipFitClassUtil.FromShipId(this.Owner.Id);
			if (shipClass == ShipFitClass.NA) return 0;

			var itemInfo = this.Item.Info;
			switch (itemInfo.Type)
			{
				case SlotItemType.大口径主砲: // 대구경주포
					if (!ShipFitClassUtil.FitTable_Heavy.ContainsKey(itemInfo.Id)) return 0; // 데이터 없음
					return ShipFitClassUtil.FitTable_Heavy[itemInfo.Id]
						?[shipClass] ?? 0;
				case SlotItemType.中口径主砲: // 중구경주포
					if (!ShipFitClassUtil.FitTable_Medium.ContainsKey(itemInfo.Id)) return 0; // 데이터 없음
					return ShipFitClassUtil.FitTable_Medium[itemInfo.Id]
						?[shipClass] ?? 0;
			}
			return 0;
		}
	}
}
