using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class Material : RawDataWrapper<kcsapi_material>
	{
		internal Material(kcsapi_material rawData) : base(rawData) { }
	}
}
