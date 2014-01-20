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
	/// 消費アイテム表します。
	/// </summary>
	public class UseItem : RawDataWrapper<kcsapi_useitem>, IIdentifiable
	{
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
			get { return this.RawData.api_name; }
		}

		public int Count
		{
			get { return this.RawData.api_count; }
		}

		internal UseItem(kcsapi_useitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Count = {2}", this.Id, this.Name, this.Count);
		}
	}
}
