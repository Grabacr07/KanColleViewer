using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace BattleInfoPlugin.Models
{
    [DataContract]
    public class MapArea
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public MapArea(kcsapi_mst_maparea maparea)
        {
            this.Id = maparea.api_id;
            this.Name = maparea.api_name;
        }

        #region static members

        private static MapArea dummy = new MapArea(new kcsapi_mst_maparea()
        {
            api_id = 0,
            api_name = "？？？",
        });

        public static MapArea Dummy
        {
            get { return dummy; }
        }

        #endregion
    }
}
