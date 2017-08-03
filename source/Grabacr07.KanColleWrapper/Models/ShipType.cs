﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦種を表します。
	/// </summary>
	public class ShipType : RawDataWrapper<kcsapi_mst_stype>, IIdentifiable
	{
		public int Id => this.RawData.api_id;

		public string Name => KanColleClient.Current.Translations.GetTranslation(RawData.api_name, TranslationType.ShipTypes, false, this.RawData, this.Id);

		public int SortNumber => this.RawData.api_sortno;

		public ShipType(kcsapi_mst_stype rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}

		#region static members

	    public static ShipType Dummy { get; } = new ShipType(new kcsapi_mst_stype
		{
		    api_id = 999,
		    api_sortno = 999,
		    api_name = "不審船",
		});

	    #endregion
	}
}
