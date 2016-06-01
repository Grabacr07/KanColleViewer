using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_slot_deprive
    {
        public kcsapi_slot_deprive_ship_data api_ship_data { get; set; }
        public kcsapi_slot_deprive_unset_List api_unset_list { get; set; }
    }

    public class kcsapi_slot_deprive_ship_data
    {
        public kcsapi_ship2 api_set_ship { get; set; }
        public kcsapi_ship2 api_unset_ship { get; set; }
    }

    public class kcsapi_slot_deprive_unset_List
    {
        public int api_type3No { get; set; }
        public int[] api_slot_list { get; set; }
    }
}
