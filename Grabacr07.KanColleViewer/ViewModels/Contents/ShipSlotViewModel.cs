using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class ShipSlotViewModel : ViewModel
    {
        public SlotItemInfo Item { get; private set; }

        public string Current { get; private set; }

        public string Maximum { get; private set; }

        public ShipSlotViewModel(Ship ship, int num)
        {
            var info = ship.SlotItems[num].Info;

            this.Item = info;
            this.Current = info.IsNumerable ? ship.OnSlot[num].ToString() : "-";
            this.Maximum = ship.Info.Slots[num].ToString();
        }
    }
}