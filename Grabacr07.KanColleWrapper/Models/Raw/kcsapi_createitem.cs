using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    // ReSharper disable InconsistentNaming
    public class kcsapi_createitem
    {
        public int api_id { get; set; }
        public int api_slotitem_id { get; set; }
        public int api_create_flag { get; set; }
        public int api_shizai_flag { get; set; }
    }
    // ReSharper restore InconsistentNaming
}