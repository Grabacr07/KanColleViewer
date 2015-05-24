using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleInfoPlugin.Models;
using Livet;

namespace BattleInfoPlugin.ViewModels.Enemies
{
    public class EnemyShipViewModel : ViewModel
    {
        public ShipData Ship { get; set; }

        public EnemyFleetViewModel ParentFleet { get; set; }
    }
}
