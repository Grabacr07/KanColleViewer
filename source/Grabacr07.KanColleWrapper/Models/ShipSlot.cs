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
		public string FitValueString
		{
			get
			{
				var fit = this.FitValue;
				if (fit == 0) return "";
				return (fit > 0 ? "+" : "-") + Math.Abs(fit);
			}
		}

		public string Tooltip => this.Item.Info.Id == 0 ? null : this.Item.NameWithLevel + "\n\n" + this.Item.Info.ToolTipData;

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
			=> ShipFitClassUtil.GetFit(this.Owner.Id, this.Item.Info.Id);
	}
}
