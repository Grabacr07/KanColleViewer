using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper;
using System.Reactive.Linq;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class AirBaseTroopViewModel : ItemViewModel
	{
		public string Name => "기지항공대";

		private AirBase[] _AirBases;
		public AirBase[] AirBases
		{
			get { return this._AirBases; }
			set
			{
				this._AirBases = value;
				this.RaisePropertyChanged();
			}
		}

		public AirBaseTroopViewModel(KanColleProxy proxy)
		{
			var homeport = KanColleClient.Current.Homeport;

			// 기항대 정보를 불러옴
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_get_member/mapinfo")
				.TryParse<kcsapi_mapinfo_airbase>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					this.AirBases = x.Data.api_air_base
						.Select(y => new AirBase(y, homeport))
						.ToArray();
				});

			// 기항대 상태 변경
			proxy.ApiSessionSource
				.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/set_action")
				.TryParse< kcsapi_empty_result>()
				.Where(x=>x.IsSuccess)
				.Subscribe(x =>
				{
					int actionKind;
					int base_id;

					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					base_id--;

					if (!int.TryParse(x.Request["api_action_kind"], out actionKind)) return;

					this.AirBases[base_id].UpdateActionKind((AirBaseAction)actionKind);
				});

			// 기항대 보급
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/supply")
				.TryParse<kcsapi_airbase_corps_supply>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					int base_id;

					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					base_id--;

					this.AirBases[base_id].Update(x.Data);
				});

			// 기항대 변경
			proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_air_corps/set_plane")
				.TryParse<kcsapi_airbase_corps_set_plane>()
				.Where(x => x.IsSuccess)
				.Subscribe(x =>
				{
					int base_id;

					if (!int.TryParse(x.Request["api_base_id"], out base_id)) return;
					base_id--;

					this.AirBases[base_id].Update(x.Data);
				});
		}
	}
}
