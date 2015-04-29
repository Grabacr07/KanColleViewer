using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 消費アイテムの種類に基づく情報を表します。
	/// </summary>
	public class UseItemInfo : RawDataWrapper<kcsapi_mst_useitem>, IIdentifiable
	{
		public int Id
		{
		    get { return this.RawData.api_id; }
		}

	    public string Name
	    {
	        get { return this.RawData.api_name; }
	    }

	    internal UseItemInfo(kcsapi_mst_useitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\"", this.Id, this.Name);
		}
	}
}
