using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace BattleInfoPlugin.Models
{
    public class ShipSlotData : NotificationObject
    {
        public SlotItemInfo Source { get; private set; }

        public int Maximum { get; private set; }

        public bool Equipped
        {
            get { return this.Source != null; }
        }

        #region Current 変更通知プロパティ

        private int _Current;

        public int Current
        {
            get { return this._Current; }
            set
            {
                if (this._Current == value) return;
                this._Current = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        public int Firepower { get; set; }
        public int Torpedo { get; set; }
        public int AA { get; set; }
        public int Armer { get; set; }
        public int Bomb { get; set; }
        public int ASW { get; set; }
        public int Hit { get; set; }
        public int Evade { get; set; }
        public int LOS { get; set; }

        public Type2 Type2 { get; set; }

        public string ToolTip
        {
            get
            {
                return (this.Firepower != 0 ? "화력:" + this.Firepower : "")
                       + (this.Torpedo != 0 ? " 뇌장:" + this.Torpedo : "")
                       + (this.AA != 0 ? " 대공:" + this.AA : "")
                       + (this.Armer != 0 ? " 장갑:" + this.Armer : "")
                       + (this.Bomb != 0 ? " 폭장:" + this.Bomb : "")
                       + (this.ASW != 0 ? " 대잠:" + this.ASW : "")
                       + (this.Hit != 0 ? " 명중:" + this.Hit : "")
                       + (this.Evade != 0 ? " 회피:" + this.Evade : "")
                       + (this.LOS != 0 ? " 색적:" + this.LOS : "")
                    ;
            }
        }

        public ShipSlotData(SlotItemInfo item, int maximum, int current)
        {
            this.Source = item;
            this.Maximum = maximum;
            this.Current = current;

            if (item == null) return;

            var m = BattleInfoPlugin.RawStart2.api_mst_slotitem.SingleOrDefault(x => x.api_id == item.Id);
            if (m == null) return;
            this.Armer = m.api_souk;
            this.Firepower = m.api_houg;
            this.Torpedo = m.api_raig;
            this.Bomb = m.api_baku;
            this.AA = m.api_tyku;
            this.ASW = m.api_tais;
            this.Hit = m.api_houm;
            this.Evade = m.api_houk;
            this.LOS = m.api_saku;
            this.Type2 = (Type2)m.api_type[1];
        }

        public ShipSlotData(ShipSlot slot)
            : this(slot.Item != null ? slot.Item.Info : null, slot.Maximum, slot.Current)
        {
        }
    }
}
