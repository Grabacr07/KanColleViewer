using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using BattleInfoPlugin.Models.Repositories;

namespace BattleInfoPlugin.Models
{
    [DataContract]
    public class MapCell
    {
        [DataMember]
        public int ColorNo { get; private set; }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public int MapInfoId { get; private set; }

        public MapInfo MapInfo { get { return Master.Current.MapInfos[this.MapInfoId] ?? MapInfo.Dummy; } }

        [DataMember]
        public int MapAreaId { get; private set; }

        [DataMember]
        public int MapInfoIdInEachMapArea { get; private set; }

        [DataMember]
        public int IdInEachMapInfo { get; private set; }

        public MapCell(Api_Mst_Mapcell cell)
        {
            this.ColorNo = cell.api_color_no;
            this.Id = cell.api_id;
            this.MapInfoId = cell.api_map_no;
            this.MapAreaId = cell.api_maparea_id;
            this.MapInfoIdInEachMapArea = cell.api_mapinfo_no;
            this.IdInEachMapInfo = cell.api_no;
        }
    }
}
