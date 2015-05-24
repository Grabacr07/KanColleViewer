using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleInfoPlugin.Models
{
    [Flags]
    public enum CellType
    {
        None = 0,

        개시 = 1 << 0,
        이벤트없음 = 1 << 1,
        보급 = 1 << 2,
        소용돌이 = 1 << 3,
        전투 = 1 << 4,
        보스 = 1 << 5,
        기분탓 = 1 << 6,  //Frameでは気のせい変更前(赤)
        항공전 = 1 << 7,
        모항 = 1 << 8,

        야전 = 1 << 31,
    }

    public static class CellTypeExtensions
    {
        public static CellType ToCellType(this int colorNo)
        {
            return (CellType)(1 << colorNo);
        }

        public static CellType ToCellType(this string battleType)
        {
            return battleType.Contains("sp_midnight") ? CellType.야전
                : battleType.Contains("airbattle") ? CellType.항공전
                : CellType.None;
        }

        public static CellType GetCellType(this MapCell cell, Dictionary<MapCell, CellType> knownTypes)
        {
            var result = CellType.None;
            if (knownTypes.ContainsKey(cell)) result = result | knownTypes[cell];
            var cellMaster = Repositories.Master.Current.MapCells[cell.Id];
            result = result | cellMaster.ColorNo.ToCellType();
            return result;
        }
    }
}
