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
using BattleInfoPlugin.ViewModels.Enemies;
using BattleInfoPlugin.Models.Repositories;

namespace BattleInfoPlugin.ViewModels
{
    public class EnemyWindowViewModel : ViewModel
    {
        public EnemyMapViewModel[] EnemyMaps { get; set; }


        #region SelectedMap変更通知プロパティ
        private EnemyMapViewModel _SelectedMap;

        public EnemyMapViewModel SelectedMap
        {
            get
            { return this._SelectedMap; }
            set
            { 
                if (this._SelectedMap == value)
                    return;
                this._SelectedMap = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        public EnemyWindowViewModel()
        {
        }

        public EnemyWindowViewModel(
            Dictionary<MapInfo, Dictionary<MapCell, Dictionary<int, FleetData>>> mapEnemies,
            Dictionary<MapCell, CellType> cellTypes)
        {
            this.EnemyMaps = Master.Current.MapInfos
                .Select(mi => new EnemyMapViewModel
                {
                    Info = mi.Value,
                    //セルポイントデータに既知の敵データを外部結合して座標でマージ
                    EnemyCells = MapResource.HasMapSwf(mi.Value)
                        ? MapResource.GetMapCellPoints(mi.Value) //マップSWFがあったらそれを元に作る
                            //外部結合
                            .GroupJoin(
                                CreateMapCellViewModelsFromEnemiesData(mi, mapEnemies, cellTypes),
                                outer => outer.Key,
                                inner => inner.Key,
                                (o, ie) => new { point = o, cells = ie })
                            .SelectMany(
                                x => x.cells.DefaultIfEmpty(),
                                (x, y) => new { x.point, cells = y })
                            //座標マージ
                            .GroupBy(x => x.point.Value)
                            .Select(x => new EnemyCellViewModel
                            {
                                Key = x.Min(y => y.point.Key), //若い番号を採用
                                EnemyFleets = x.Where(y => y.cells != null) //敵データをEnemyIdでマージ
                                    .SelectMany(y => y.cells.EnemyFleets)
                                    .GroupBy(y => y.Key)
                                    .Select(y => y.First())
                                    .ToArray(),
                                ColorNo = x.Where(y => y.cells != null).Select(y => y.cells.ColorNo).FirstOrDefault(),
                                CellType = x.Where(y => y.cells != null).Select(y => y.cells.CellType).FirstOrDefault(),
                            })
                            //敵データのないセルは除外
                            .Where(x => x.EnemyFleets.Any())
                            .ToArray()
                        : CreateMapCellViewModelsFromEnemiesData(mi, mapEnemies, cellTypes) //なかったら敵データだけ(重複るが仕方ない)
                            .OrderBy(cell => cell.Key)
                            .ToArray(),
                })
                .OrderBy(info => info.Info.Id)
                .ToArray();

        }

        private static IEnumerable<EnemyCellViewModel> CreateMapCellViewModelsFromEnemiesData(
            KeyValuePair<int, MapInfo> mi,
            Dictionary<MapInfo, Dictionary<MapCell, Dictionary<int, FleetData>>> mapEnemies,
            Dictionary<MapCell, CellType> cellTypes)
        {
            return mapEnemies.Where(info => info.Key.Id == mi.Key)
                .Select(info => info.Value)
                .SelectMany(cells => cells)
                .Select(cell => new EnemyCellViewModel
                {
                    Key = cell.Key.IdInEachMapInfo,
                    EnemyFleets = cell.Value
                        .Select(enemy => new EnemyFleetViewModel
                        {
                            Key = enemy.Key,
                            Fleet = enemy.Value,
                            EnemyShips = enemy.Value.Ships.Select(s => new EnemyShipViewModel { Ship = s }).ToArray(),
                        })
                        .OrderBy(enemy => enemy.Key)
                        .ToArray(),
                    ColorNo = cell.Key.ColorNo,
                    CellType = cell.Key.GetCellType(cellTypes),
                });
        }

        public void Initialize()
        {
        }
    }
}
