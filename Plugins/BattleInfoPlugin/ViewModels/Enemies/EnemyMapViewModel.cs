using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using BattleInfoPlugin.Models;
using BattleInfoPlugin.Models.Repositories;
using Grabacr07.KanColleViewer.ViewModels;

namespace BattleInfoPlugin.ViewModels.Enemies
{
    public class EnemyMapViewModel : TabItemViewModel
    {
        public MapInfo Info { get; set; }

        #region EnemyCells

        private EnemyCellViewModel[] _EnemyCells;

        public EnemyCellViewModel[] EnemyCells
        {
            get { return this._EnemyCells; }
            set
            {
                this._EnemyCells = value;
                if (value == null) return;
                foreach (var val in value)
                {
                    val.ParentMap = this;
                }
            }
        }

        #endregion

        public IEnumerable<EnemyShipViewModel> EnemyShips
        {
            get
            {
                return this.EnemyCells.SelectMany(x => x.EnemyFleets).SelectMany(x => x.EnemyShips);
            }
        }

        public BitmapSource MapImage { get { return MapResource.GetMapImage(this.Info); } }

        public bool HasImage { get { return this.MapImage != null; } }

        public bool ExistsMapAssembly { get { return MapResource.ExistsAssembly; } }

        public IDictionary<string, Tuple<Point, int>> CellPoints
        {
            get
            {
                return MapResource.GetMapCellPoints(this.Info)
                    .GroupBy(kvp => kvp.Value) //重複ポイントを除去
                    .Select(g => g.OrderBy(x => x.Key).First())
                    .ToDictionary(
                        x => x.Key.ToString(),
                        x => Tuple.Create(x.Value,
                            Master.Current.MapCells.Select(c => c.Value).Single(c => c.IdInEachMapInfo == x.Key && c.MapInfoId == this.Info.Id).ColorNo)
                    );
            }
        }

        public override string Name
        {
            get
            {
                return this.MapNo + ": " + this.Info.Name;
            }
            protected set { throw new NotImplementedException(); }
        }

        public string MapNo
        {
            get { return this.Info.MapAreaId + "-" + this.Info.IdInEachMapArea; }
        }

        public string RequiredDefeatCount
        {
            get { return 21 < this.Info.MapAreaId ? "Event" : this.Info.RequiredDefeatCount.ToString(); }
        }
    }
}
