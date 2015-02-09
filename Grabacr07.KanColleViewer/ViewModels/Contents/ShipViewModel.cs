using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class ShipViewModel : ViewModel
    {
        public Ship Ship { get; private set; }

        public ShipSlotViewModel[] Slots { get; }

        public ShipViewModel(Ship ship)
        {
            this.Ship = ship;
            this.Slots = ship.SlotItems.Select((_, i) => new ShipSlotViewModel(ship, i)).ToArray();
        }
    }
}
