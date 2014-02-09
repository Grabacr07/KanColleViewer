using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.IO;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 装備アイテムの種類に基づく情報を表します。
	/// </summary>
	public class SlotItemInfo : RawDataWrapper<kcsapi_master_slotitem>, IIdentifiable
	{
		private SlotItemIconType? iconType;
		private int? categoryId;

		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
            //원본코드
			//get { return this.RawData.api_name; }
            //commit참고.https://github.com/Zharay/KanColleViewer/commit/ba21e509635aa59343b1070abd97702d1b060eb4
            //수정코드.https://github.com/Zharay/KanColleViewer/blob/ba21e509635aa59343b1070abd97702d1b060eb4/Grabacr07.KanColleWrapper/Models/SlotItemInfo.cs
            get
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                string Main_folder = Path.GetDirectoryName(location);
                if (System.IO.File.Exists(Main_folder + "\\equipment.txt") == true)
                {
                    System.IO.StreamReader filereader = new System.IO.StreamReader(Main_folder + "\\equipment.txt", System.Text.Encoding.UTF8, true);
                    string read_line = null;
                    string jap_name = null;
                    string eng_name = null;
                    while (true)
                    {
                        read_line = filereader.ReadLine();
                        if (String.IsNullOrEmpty(read_line)) { filereader.Close(); break; }
                        else
                        {
                            char[] delimiter = { ';', ',' };
                            jap_name = read_line.Split(delimiter)[0];
                            eng_name = read_line.Split(delimiter)[1];
                            if (String.Equals(RawData.api_name, jap_name))
                            { filereader.Close(); return eng_name; }
                        }
                    }
                    return this.RawData.api_name;
                }
                else
                {
                    return this.RawData.api_name;
                }
            }
		}

		public SlotItemIconType IconType
		{
			get { return this.iconType ?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0)); }
		}

		public int CategoryId
		{
			get { return this.categoryId ?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue); }
		}

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA
		{
			get { return this.RawData.api_tyku; }
		}

		/// <summary>
		/// この装備アイテムが艦載機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAircraft
		{
			get
			{
				return this.IconType == SlotItemIconType.Fighter ||
					   this.IconType == SlotItemIconType.TorpedoBomber ||
					   this.IconType == SlotItemIconType.DiveBomber ||
					   this.IconType == SlotItemIconType.ReconPlane;
			}
		}

		/// <summary>
		/// この装備アイテムが水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsSeaplane
		{
			get { return this.IconType == SlotItemIconType.ReconSeaplane; }
		}

		internal SlotItemInfo(kcsapi_master_slotitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Type = {{{2}}}", this.Id, this.Name, this.RawData.api_type.ToString(", "));
		}

		#region static members

		private static readonly SlotItemInfo dummy = new SlotItemInfo(new kcsapi_master_slotitem()
		{
			api_id = 0,
			api_name = "？？？",
		});

		public static SlotItemInfo Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
