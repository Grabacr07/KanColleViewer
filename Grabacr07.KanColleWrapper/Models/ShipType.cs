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
	/// 艦種を表します。
	/// </summary>
	public class ShipType : RawDataWrapper<kcsapi_stype>, IIdentifiable
	{
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
            //원본코드
			//get { return this.RawData.api_name; }
            //commit발췌.https://github.com/Zharay/KanColleViewer/commit/ba21e509635aa59343b1070abd97702d1b060eb4
            //수정코드.https://github.com/Zharay/KanColleViewer/blob/ba21e509635aa59343b1070abd97702d1b060eb4/Grabacr07.KanColleWrapper/Models/ShipType.cs
            get
            {
                // Translate IJN ship types to the USN ones.
                if (RawData.api_name == "水上機母艦") { return string.Format("수상모"); }
                if (RawData.api_name == "戦艦") { return string.Format("전함"); }
                if (RawData.api_name == "航空戦艦") { return string.Format("항전"); }
                if (RawData.api_name == "重巡洋艦") { return string.Format("중순"); }
                if (RawData.api_name == "航空巡洋艦") { return string.Format("항순"); }
                if (RawData.api_name == "軽巡洋艦") { return string.Format("경순"); }
                if (RawData.api_name == "重雷装巡洋艦") { return string.Format("중뢰순"); }
                if (RawData.api_name == "正規空母") { return string.Format("항모"); }
                if (RawData.api_name == "軽空母") { return string.Format("경항모"); }
                if (RawData.api_name == "駆逐艦") { return string.Format("구축함"); }
                if (RawData.api_name == "潜水艦") { return string.Format("잠수함"); }
                if (RawData.api_name == "潜水空母") { return string.Format("잠항모"); }
                if (RawData.api_name == "装甲空母") { return string.Format("장갑모"); }
                if (RawData.api_name == "揚陸艦") { return string.Format("양륙함"); }
                if (RawData.api_name == "海防艦") { return string.Format("해방함"); }
                if (RawData.api_name == "超弩級戦艦") { return string.Format("초노급"); }
                if (RawData.api_name == "補給艦") { return string.Format("보급함"); }

                return this.RawData.api_name;
            }
		}

		public int SortNumber
		{
			get { return this.RawData.api_sortno; }
		}

		public ShipType(kcsapi_stype rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\"", this.Id, this.Name);
		}

		#region static members

		private static readonly ShipType dummy = new ShipType(new kcsapi_stype
		{
			api_id = 999,
			api_sortno = 999,
			api_name = "不審船",
		});

		public static ShipType Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
