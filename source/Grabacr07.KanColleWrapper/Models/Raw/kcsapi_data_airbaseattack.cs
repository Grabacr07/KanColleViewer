using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_data_airbaseattack
    {
        public int api_base_id { get; set; }
        public int[] api_stage_flag { get; set; }
        public int[][] api_plane_from { get; set; }
        public kcsapi_data_squadron_plane[] api_squadron_plane { get; set; }
        public kcsapi_data_stage1 api_stage1 { get; set; }
        public kcsapi_data_stage2 api_stage2 { get; set; }
        public kcsapi_data_stage3 api_stage3 { get; set; }  // e のみのデータ
    }
    public class kcsapi_data_squadron_plane
    {
        public int api_mst_id { get; set; }
        public int api_count { get; set; }
    }
}
