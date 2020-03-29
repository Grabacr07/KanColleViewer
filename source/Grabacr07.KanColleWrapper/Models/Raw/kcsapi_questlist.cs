﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_questlist
	{
		public int api_count { get; set; }
		public int api_completed_kind { get; set; }
		public int api_exec_count { get; set; }
		public kcsapi_quest[] api_list { get; set; }
		public int api_exec_type { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
