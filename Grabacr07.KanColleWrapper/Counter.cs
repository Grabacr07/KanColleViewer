using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public abstract class CounterBase : NotificationObject
	{
		#region Count 変更通知プロパティ

		private int _Count;

		public int Count
		{
			get { return this._Count; }
			set
			{
				if (this._Count != value)
				{
					this._Count = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public void Reset()
		{
			this.Count = 0;
		}
	}

	public class ItemDestroyCounter : CounterBase
	{
		public ItemDestroyCounter(KanColleProxy proxy)
		{
			proxy.api_req_kousyou_destroyitem2
				.TryParse()
				.Where(x => x.IsSuccess)
				.Subscribe(_ => this.Count++);
		}
	}

	public class SupplyCounter : CounterBase
	{
		public SupplyCounter(KanColleProxy proxy)
		{
			proxy.api_req_hokyu_charge
				.TryParse()
				.Where(x => x.IsSuccess)
				.Subscribe(_ => this.Count++);
		}
	}


	public class MissionCounter : CounterBase
	{
		public MissionCounter(KanColleProxy proxy)
		{
			proxy.api_req_mission_result
				.TryParse<kcsapi_mission_result>()
				.Where(x => x.IsSuccess)
				.Where(x => x.Data.api_clear_result == 1)
				.Subscribe(_ => this.Count++);
		}
	}
}
