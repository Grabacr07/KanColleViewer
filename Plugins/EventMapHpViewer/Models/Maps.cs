using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;

namespace EventMapHpViewer.Models
{
    public class Maps
    {
        public MapData[] MapList { get; set; }
        public static MasterTable<MapArea> MapAreas { get; set; }
        public static MasterTable<MapInfo> MapInfos { get; set; }
    }

    public class MapData
    {
        public int Id { get; set; }
        public int IsCleared { get; set; }
        public int IsExBoss { get; set; }
        public int DefeatCount { get; set; }
        public Eventmap Eventmap { get; set; }

        public MapInfo Master
        {
            get { return Maps.MapInfos[this.Id]; }
        }

        public string MapNumber
        {
            get
            {
                return this.Master.MapAreaId + "-" + this.Master.IdInEachMapArea;
            }
        }

        public string Name
        {
            get { return this.Master.Name; }
        }

        public string AreaName
        {
            get { return this.Master.MapArea.Name; }
        }

        public int Max
        {
            get
            {
                if (this.IsExBoss == 1)
                    return this.Master.RequiredDefeatCount;
                if (this.Eventmap != null) return this.Eventmap.MaxMapHp;
                return 1;
            }
        }

        public int Current
        {
            get
            {
                if (this.IsExBoss == 1)　return this.Master.RequiredDefeatCount - this.DefeatCount;  //ゲージ有り通常海域
                
                if (this.Eventmap != null) return this.Eventmap.NowMapHp;   //イベント海域

                return 1;   //ゲージ無し通常海域
            }
        }

        public int RemainingCount
        {
            get
            {
                if (this.IsExBoss == 1)
                {
                    return this.Current;    //ゲージ有り通常海域
                }

                if (this.Eventmap == null) return 1;    //ゲージ無し通常海域

                var shipMaster = KanColleClient.Current.Master.Ships;
                try
                {
                    var lastBossHp = shipMaster[EventBossDictionary[this.Eventmap.SelectedRank][this.Id].Last()].HP;
                    var normalBossHp = shipMaster[EventBossDictionary[this.Eventmap.SelectedRank][this.Id].First()].HP;
                    if (this.Current <= lastBossHp) return 1;   //最後の1回
                    return (int)Math.Ceiling((double)(this.Current - lastBossHp) / normalBossHp) + 1;   //イベント海域
                }
                catch (KeyNotFoundException)
                {
                    return -1;  //未対応
                }
            }
        }

        public static readonly IReadOnlyDictionary<int, IReadOnlyDictionary<int, int[]>> EventBossDictionary
            = new Dictionary<int, IReadOnlyDictionary<int, int[]>>
            {
                { //難易度未選択
                    0, new Dictionary<int, int[]>
                    {
                        { 271, new[] { 566 } },
                        { 272, new[] { 581, 582 } },
                        { 273, new[] { 585 } },
                        { 274, new[] { 583, 584 } },
                        { 275, new[] { 586 } },
                        { 276, new[] { 557 } },

                        { 281, new[] { 595 } },
                        { 282, new[] { 597, 598 } },
                        { 283, new[] { 557 } },
                        { 284, new[] { 599, 600 } },
                    }
                },
                { //丙
                    1, new Dictionary<int, int[]>
                    {
                        { 291, new[] { 570, 571 } },
                        { 292, new[] { 528, 565 } },
                        { 293, new[] { 601 } },
                        { 294, new[] { 586 } },
                        { 295, new[] { 603 } },
                        { 301, new[] { 566 } },
                        { 302, new[] { 545 } },
                        { 303, new[] { 558 } },
                        { 304, new[] { 605, 607 } },
                        { 305, new[] { 609, 611 } },
                        { 306, new[] { 603 } },
                    }
                },
                { //乙
                    2, new Dictionary<int, int[]>
                    {
                        { 291, new[] { 571 } },
                        { 292, new[] { 528, 565 } },
                        { 293, new[] { 601, 602 } },
                        { 294, new[] { 586 } },
                        { 295, new[] { 604 } },
                        { 301, new[] { 566 } },
                        { 302, new[] { 545 } },
                        { 303, new[] { 558 } },
                        { 304, new[] { 606, 608 } },
                        { 305, new[] { 609, 611 } },
                        { 306, new[] { 603 } },
                    }
                },
                { //甲
                    3, new Dictionary<int, int[]>
                    {
                        { 291, new[] { 571, 572 } },
                        { 292, new[] { 579, 565 } },
                        { 293, new[] { 602 } },
                        { 294, new[] { 586 } },
                        { 295, new[] { 604 } },
                        { 301, new[] { 566 } },
                        { 302, new[] { 545 } },
                        { 303, new[] { 558 } },
                        { 304, new[] { 606, 608 } },
                        { 305, new[] { 610, 612 } },
                        { 306, new[] { 604 } },
                    }
                },
            };
    }

    public class Eventmap
    {
        public int NowMapHp { get; set; }
        public int MaxMapHp { get; set; }
        public int State { get; set; }
        public int SelectedRank { get; set; }
    }
}
