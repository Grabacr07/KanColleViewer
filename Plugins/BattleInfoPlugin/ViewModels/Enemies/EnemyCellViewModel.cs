using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using BattleInfoPlugin.Models;

namespace BattleInfoPlugin.ViewModels.Enemies
{
    public class EnemyCellViewModel : ViewModel
    {
        public int Key { get; set; }

        #region EnemyFleets

        private EnemyFleetViewModel[] _EnemyFleets;

        public EnemyFleetViewModel[] EnemyFleets
        {
            get { return this._EnemyFleets; }
            set
            {
                this._EnemyFleets = value;
                if (value == null) return;
                foreach (var val in value)
                {
                    val.ParentCell = this;
                }
            }
        }

        #endregion

        public CellType CellType { get; set; }

        public int ColorNo { get; set; }

        public EnemyMapViewModel ParentMap { get; set; }
    }
}
