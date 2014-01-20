using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 資源および資材の所有状況を表します。
	/// </summary>
	public class Materials
	{
		private readonly Material[] materials;

		/// <summary>
		/// 所有している燃料数を取得します。
		/// </summary>
		public int Fuel
		{
			get { return this.Get(0); }
		}

		/// <summary>
		/// 所有している弾薬数を取得します。
		/// </summary>
		public int Ammunition
		{
			get { return this.Get(1); }
		}

		/// <summary>
		/// 所有している鉄鋼数を取得します。
		/// </summary>
		public int Steel
		{
			get { return this.Get(2); }
		}

		/// <summary>
		/// 所有しているボーキサイト数を取得します。
		/// </summary>
		public int Bauxite
		{
			get { return this.Get(3); }
		}

		/// <summary>
		/// 所有している開発資材の数を取得します。
		/// </summary>
		public int DevelopmentMaterials
		{
			get { return this.Get(6); }
		}

		/// <summary>
		/// 所有している高速修復材の数を取得します。
		/// </summary>
		public int InstantRepairMaterials
		{
			get { return this.Get(5); }
		}

		/// <summary>
		/// バケツ！！！ ぶっかけ！！！！
		/// </summary>
		public int Bucket
		{
			get { return this.InstantRepairMaterials; }
		}

		/// <summary>
		/// 所有している高速建造材の数を取得します。
		/// </summary>
		public int InstantBuildMaterials
		{
			get { return this.Get(4); }
		}


		internal Materials(Material[] materials)
		{
			this.materials = materials;
		}

		private int Get(int index)
		{
			if (this.materials != null && this.materials.Length == 7)
			{
				return this.materials[index].RawData.api_value;
			}

			return 0;
		}
	}
}
