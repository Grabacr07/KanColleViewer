using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class AirBase : RawDataWrapper<kcsapi_airbase_corps>, IIdentifiable
	{
		private Homeport homeport { get; }

		public int Id => this.RawData.api_rid;
		public string Name => this.RawData.api_name;
		public int Distance => this.RawData.api_distance;
		public AirBaseAction ActionKind => (AirBaseAction)this.RawData.api_action_kind;

		public AirBasePlane[] Planes { get; private set; }
		public int AirSuperiorityPotential { get; private set; }

		public AirBase(kcsapi_airbase_corps rawData, Homeport homeport) : base(rawData)
		{
			this.homeport = homeport;
			this.Update(RawData);
		}

		public void Update(kcsapi_airbase_corps rawData)
		{
			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
		}
		public void Update(kcsapi_airbase_corps_set_plane rawData)
		{
			this.RawData.api_distance = rawData.api_distance;

			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
		}
		public void Update(kcsapi_airbase_corps_supply rawData)
		{
			this.RawData.api_distance = rawData.api_distance;

			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
		}
		public void UpdateActionKind(AirBaseAction action)
		{
			this.RawData.api_action_kind = (int)action;
		}
	}

	public class AirBasePlane : RawDataWrapper<kcsapi_plane_info>, IIdentifiable
	{
		public SlotItem Source { get; }

		public int Id => this.RawData.api_squadron_id;
		public string Name => this.Source?.NameWithLevel ?? "";
		public int State => this.RawData.api_state;

		public int MaxCount => this.RawData.api_max_count;
		public int CurrentCount => this.RawData.api_count;
		public int LostCount => this.MaxCount - this.CurrentCount;

		public ConditionType Condition => ConditionTypeHelper.ToConditionType(this.RawData.api_cond);

		public AirBasePlane(kcsapi_plane_info rawData, SlotItem slotitem) : base(rawData)
		{
			this.Source = slotitem;
		}
	}

	public enum AirBaseAction
	{
		대기 = 0,
		출격 = 1,
		방공 = 2,
		퇴피 = 3,
		휴식 = 4
	}
}
