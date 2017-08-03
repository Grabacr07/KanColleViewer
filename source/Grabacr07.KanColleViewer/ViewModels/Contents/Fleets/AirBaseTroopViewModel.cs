﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper;
using Livet;
using System.Reactive.Linq;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class AirBaseTroopViewModel : ItemViewModel
	{
		public string Name => "기항대";

		#region AirBases 변경통지 프로퍼티
		private Dictionary<int, Dictionary<int, AirBase>> _AirBases;
		public Dictionary<int, Dictionary<int, AirBase>> AirBases
		{
			get { return this._AirBases; }
			set
			{
				this._AirBases = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region IsNotinitialized 변경통지 프로퍼티
		private bool _IsNotinitialized;
		public bool IsNotinitialized
		{
			get { return this._IsNotinitialized; }
			private set
			{
				this._IsNotinitialized = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		public Visibility IsFirstFleet => Visibility.Collapsed;
		public bool IsNotFleet => true;

		public ViewModel QuickStateView => AirBaseStateViewModel.Instance;


		public AirBaseTroopViewModel(KanColleProxy proxy)
		{
			var homeport = KanColleClient.Current.Homeport;
			this.IsNotinitialized = true;

			// 기항대 정보를 불러옴
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_get_member/mapinfo")
				.TryParse<kcsapi_mapinfo_airbase>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					this.IsNotinitialized = false;

					this.AirBases = x.Data.api_air_base?
						.Select(y => new AirBase(y, homeport))
						.GroupBy(y => y.RawData.api_area_id)
						.ToDictionary(y => y.Key, y => y.ToDictionary(z => z.RawData.api_rid, z => z));
				});

			// 기항대 상태 변경
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/set_action")
				.TryParse< kcsapi_empty_result>()
				.Where(x=>x.IsSuccess)
				.Subscribe(x =>
				{
					int actionKind;
					int area_id, base_id;

					if (!int.TryParse(x.Request["api_area_id"], out area_id)) return;
					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					if (!this.AirBases.ContainsKey(area_id) || !this.AirBases[area_id].ContainsKey(base_id)) return;

					if (!int.TryParse(x.Request["api_action_kind"], out actionKind)) return;

					this.AirBases[area_id][base_id].UpdateActionKind((AirBaseAction)actionKind);
					this.RaisePropertyChanged(nameof(this.AirBases));
				});

			// 기항대 보급
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/supply")
				.TryParse<kcsapi_airbase_corps_supply>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					int area_id, base_id;

					if (!int.TryParse(x.Request["api_area_id"], out area_id)) return;
					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					if (!this.AirBases.ContainsKey(area_id) || !this.AirBases[area_id].ContainsKey(base_id)) return;

					this.AirBases[area_id][base_id].Update(x.Data);
					this.RaisePropertyChanged(nameof(this.AirBases));
				});

			// 기항대 변경
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/set_plane")
				.TryParse<kcsapi_airbase_corps_set_plane>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					int area_id, base_id;

					if (!int.TryParse(x.Request["api_area_id"], out area_id)) return;
					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					if (!this.AirBases.ContainsKey(area_id) || !this.AirBases[area_id].ContainsKey(base_id)) return;

					this.AirBases[area_id][base_id].Update(x.Data);
					this.RaisePropertyChanged(nameof(this.AirBases));
				});
		}
	}
}
