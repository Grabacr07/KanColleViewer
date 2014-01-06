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
	public class Fleet : NotificationObject, IDisposable, IIdentifiable
	{
		private readonly Homeport homeport;

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

		#region AverageLevel 変更通知プロパティ

		private double _AverageLevel;

		/// <summary>
		/// 艦隊の平均レベルを取得します。
		/// </summary>
		public double AverageLevel
		{
			get { return this._AverageLevel; }
			private set
			{
				if (this._AverageLevel != value)
				{
					this._AverageLevel = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AirSuperiorityPotential 変更通知プロパティ

		private int _AirSuperiorityPotential;

		/// <summary>
		/// 艦隊の制空能力を取得します。
		/// </summary>
		public int AirSuperiorityPotential
		{
			get { return this._AirSuperiorityPotential; }
			private set
			{
				if (this._AirSuperiorityPotential != value)
				{
					this._AirSuperiorityPotential = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Speed 変更通知プロパティ

		private Speed _Speed;

		/// <summary>
		/// 艦隊の速力を取得します。
		/// </summary>
		public Speed Speed
		{
			get { return this._Speed; }
			private set
			{
				if (this._Speed != value)
				{
					this._Speed = value;
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

		public FleetReSortie ReSortie { get; private set; }
		public Expedition Expedition { get; private set; }

		internal Fleet(Homeport parent, kcsapi_deck rawData)
		{
			this.homeport = parent;

			this.ReSortie = new FleetReSortie();
			this.Expedition = new Expedition(this);
			this.Update(rawData);
		}


		internal void Update(kcsapi_deck rawData)
		{
			this.Id = rawData.api_id;
			this.Name = rawData.api_name;
			this.Ships = rawData.api_ship.Select(id => this.homeport.Ships[id]).Where(x => x != null).ToArray();
			this.AverageLevel = this.Ships.HasValue() ? this.Ships.Average(s => s.Level) : 0.0;
			this.AirSuperiorityPotential = this.Ships.Sum(s => s.CalcAirSuperiorityPotential());
			this.Speed = this.Ships.All(s => s.Info.Speed == Speed.Fast) ? Speed.Fast : Speed.Low;
			this.ReSortie.Update(this.Ships);
			this.Expedition.Update(rawData.api_mission);

			this.UpdateStatus();
		}

		internal void UpdateStatus()
		{
			if (this.Ships.Length == 0) this.State = FleetState.Empty;
			else if (this.Expedition.IsInExecution) this.State = FleetState.Expedition;
			else if (this.homeport.Repairyard.CheckRepairing(this)) this.State = FleetState.Repairing;
			else this.State = FleetState.Ready;
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Ships = {2}", this.Id, this.Name, this.GetShips().Select(s => "\"" + s.Info.Name + "\"").ToString(","));
		}

		public virtual void Dispose()
		{
			this.ReSortie.SafeDispose();
			this.Expedition.SafeDispose();
		}
	}
}
