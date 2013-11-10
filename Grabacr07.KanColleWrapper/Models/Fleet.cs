using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper.Models
{
	public class Fleet : NotificationObject, IIdentifiable
	{
		#region Id 変更通知プロパティ

		private int _Id;

		public int Id
		{
			get { return this._Id; }
			set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Ships 変更通知プロパティ

		private Ship[] _Ships = new Ship[0];

		/// <summary>
		/// 艦隊に所属している艦娘 (空いている枠は null) の配列を取得します。
		/// </summary>
		public Ship[] Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region State 変更通知プロパティ

		private FleetState _State;

		public FleetState State
		{
			get { return this._State; }
			set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public Expedition Expedition { get; private set; }


		internal Fleet(kcsapi_deck rawData)
		{
			this.Expedition = new Expedition(this);
			this.Update(rawData);
		}


		internal void Update(kcsapi_deck rawData)
		{
			this.Id = rawData.api_id;
			this.Name = rawData.api_name;
			this.Ships = rawData.api_ship.Select(id => KanColleClient.Current.Homeport.Ships[id]).Where(x => x != null).ToArray();
			this.Expedition.Update(rawData.api_mission);

			if (this.Expedition.IsInExecution) this.State = FleetState.Expedition;
			else if(KanColleClient.Current.Homeport.Repairyard.CheckRepairing(this)) this.State = FleetState.Repairing;
			else this.State = FleetState.Ready;
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Ships = {2}", this.Id, this.Name, this.GetShips().Select(s => "\"" + s.Info.Name + "\"").ToString(","));
		}
	}
}
