﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Reactive.Linq;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 資源および資材の保有状況を表します。
	/// </summary>
	public class Materials : Notifier
	{
		#region Fuel 変更通知プロパティ

		private int _Fuel;

		/// <summary>
		/// 所有している燃料数を取得します。
		/// </summary>
		public int Fuel
		{
			get { return this._Fuel; }
			private set
			{
				if (this._Fuel != value)
				{
					this._Fuel = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Ammunition 変更通知プロパティ

		private int _Ammunition;

		/// <summary>
		/// 所有している弾薬数を取得します。
		/// </summary>
		public int Ammunition
		{
			get { return this._Ammunition; }
			private set
			{
				if (this._Ammunition != value)
				{
					this._Ammunition = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Steel 変更通知プロパティ

		private int _Steel;

		/// <summary>
		/// 所有している鉄鋼数を取得します。
		/// </summary>
		public int Steel
		{
			get { return this._Steel; }
			private set
			{
				if (this._Steel != value)
				{
					this._Steel = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Bauxite 変更通知プロパティ

		private int _Bauxite;

		/// <summary>
		/// 所有しているボーキサイト数を取得します。
		/// </summary>
		public int Bauxite
		{
			get { return this._Bauxite; }
			private set
			{
				if (this._Bauxite != value)
				{
					this._Bauxite = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region DevelopmentMaterials 変更通知プロパティ

		private int _DevelopmentMaterials;

		/// <summary>
		/// 所有している開発資材の数を取得します。
		/// </summary>
		public int DevelopmentMaterials
		{
			get { return this._DevelopmentMaterials; }
			private set
			{
				if (this._DevelopmentMaterials != value)
				{
					this._DevelopmentMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region InstantRepairMaterials 変更通知プロパティ

		private int _InstantRepairMaterials;

		/// <summary>
		/// 所有している高速修復材の数を取得します。
		/// </summary>
		public int InstantRepairMaterials
		{
			get { return this._InstantRepairMaterials; }
			private set
			{
				if (this._InstantRepairMaterials != value)
				{
					this._InstantRepairMaterials = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("Bucket");
				}
			}
		}

		/// <summary>
		/// バケツ！！！ ぶっかけ！！！！
		/// </summary>
		public int Bucket => this.InstantRepairMaterials;

		#endregion

		#region InstantBuildMaterials 変更通知プロパティ

		private int _InstantBuildMaterials;

		/// <summary>
		/// 所有している高速建造材の数を取得します。
		/// </summary>
		public int InstantBuildMaterials
		{
			get { return this._InstantBuildMaterials; }
			private set
			{
				if (this._InstantBuildMaterials != value)
				{
					this._InstantBuildMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ImprovementMaterials 変更通知プロパティ

		private int _ImprovementMaterials;

		public int ImprovementMaterials
		{
			get { return this._ImprovementMaterials; }
			set
			{
				if (this._ImprovementMaterials != value)
				{
					this._ImprovementMaterials = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Materials(KanColleProxy proxy)
		{
			proxy.api_get_member_material.TryParse<kcsapi_material[]>().Subscribe(x => this.Update(x.Data));
			proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => this.Update(x.Data.api_material));
			proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => this.Update(x.Data.api_material));

			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/supply")
				.TryParse<kcsapi_airbase_corps_supply>()
				.Where(x => x.IsSuccess)
				.Subscribe(x => this.Update(new int[] { x.Data.api_after_fuel, this.Ammunition, this.Steel, x.Data.api_after_bauxite }));

			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/set_plane")
				.TryParse<kcsapi_airbase_corps_set_plane>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					if (x.Request["api_item_id"] == "-1") return;
					if (x.Data.api_plane_info.Length >= 2) return;

					this.Update(new int[] { this.Fuel, this.Ammunition, this.Steel, x.Data.api_after_bauxite });
				});

			proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => this.Update(x.Data.api_material));
		}


		internal void Update(kcsapi_material[] source)
		{
			if (source != null && 8 <= source.Length)
			{
				this.Fuel = source[0].api_value;
				this.Ammunition = source[1].api_value;
				this.Steel = source[2].api_value;
				this.Bauxite = source[3].api_value;
				this.DevelopmentMaterials = source[6].api_value;
				this.InstantRepairMaterials = source[5].api_value;
				this.InstantBuildMaterials = source[4].api_value;
				this.ImprovementMaterials = source[7].api_value;
			}
		}

		private void Update(int[] source)
		{
			if (source != null && source.Length == 4)
			{
				this.Fuel = source[0];
				this.Ammunition = source[1];
				this.Steel = source[2];
				this.Bauxite = source[3];
			}
		}
	}
}
