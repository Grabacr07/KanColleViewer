using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;
using Livet;
using Livet.EventListeners.WeakEvents;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 複数の艦娘によって編成される艦隊を表します。
	/// </summary>
	public class Fleet : NotificationObject, IDisposable, IIdentifiable
	{
		private readonly Homeport homeport;
		private Ship[] originalShips; // null も含んだやつ
		private bool isInSortie;
		private LivetCompositeDisposable compositeDisposable;

		#region Id 変更通知プロパティ

		private int _Id;

		public int Id
		{
			get { return this._Id; }
			private set
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
			internal set
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
		/// 艦隊に所属している艦娘の配列を取得します。
		/// </summary>
		public Ship[] Ships
		{
			get { return this._Ships; }
			private set
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
				if (!this._AverageLevel.Equals(value))
				{
					this._AverageLevel = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TotalLevel 変更通知プロパティ

		private int _TotalLevel;

		public int TotalLevel
		{
			get { return this._TotalLevel; }
			private set
			{
				if (this._TotalLevel != value)
				{
					this._TotalLevel = value;
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

		#region TotalViewRange 変更通知プロパティ

		private int _TotalViewRange;

		/// <summary>
		/// 各艦娘の装備によるボーナスを含めた、艦隊の索敵合計値を取得します。
		/// </summary>
		public int TotalViewRange
		{
			get { return this._TotalViewRange; }
			private set
			{
				if (this._TotalViewRange != value)
				{
					this._TotalViewRange = value;
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
			private set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// 艦隊に編成されている艦娘のコンディションを取得します。
		/// </summary>
		public FleetCondition Condition { get; private set; }

		/// <summary>
		/// 艦隊の遠征に関するステータスを取得します。
		/// </summary>
		public Expedition Expedition { get; private set; }

		#region IsReady 変更通知プロパティ

		private bool _IsReady;

		/// <summary>
		/// 艦隊の出撃準備ができているかどうかを示す値を取得します。
		/// </summary>
		public bool IsReady
		{
			get { return this._IsReady; }
			private set
			{
				if (this._IsReady != value)
				{
					this._IsReady = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsWounded 変更通知プロパティ

		private bool _IsWounded;

		/// <summary>
		/// 艦隊に大破した艦娘がいるかどうかを示す値を取得します。
		/// </summary>
		public bool IsWounded
		{
			get { return this._IsWounded; }
			private set
			{
				if (this._IsWounded != value)
				{
					this._IsWounded = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsInShortSupply 変更通知プロパティ

		private bool _IsInShortSupply;

		/// <summary>
		/// 艦隊に完全に補給されていない艦娘がいるかどうかを示す値を取得します。
		/// </summary>
		public bool IsInShortSupply
		{
			get { return this._IsInShortSupply; }
			private set
			{
				if (this._IsInShortSupply != value)
				{
					this._IsInShortSupply = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsRepairling 変更通知プロパティ

		private bool _IsRepairling;

		/// <summary>
		/// 艦隊に入渠中の艦娘がいるかどうかを示す値を取得します。
		/// </summary>
		public bool IsRepairling
		{
			get { return this._IsRepairling; }
			private set
			{
				if (this._IsRepairling != value)
				{
					this._IsRepairling = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Fleet(Homeport parent, kcsapi_deck rawData)
		{
			this.homeport = parent;

			this.Condition = new FleetCondition(this);
			this.Expedition = new Expedition(this);
			this.Update(rawData);

			this.compositeDisposable = new LivetCompositeDisposable
			{
				new PropertyChangedWeakEventListener(KanColleClient.Current.Settings)
				{
					{ "ViewRangeCalcLogic", (sender, args) => this.Calculate() }
				}
			};
		}


		/// <summary>
		/// 指定した <see cref="kcsapi_deck"/> を使用して艦隊の情報をすべて更新します。
		/// </summary>
		/// <param name="rawData">エンド ポイントから取得したデータ。</param>
		internal void Update(kcsapi_deck rawData)
		{
			this.Id = rawData.api_id;
			this.Name = rawData.api_name;

			this.Expedition.Update(rawData.api_mission);
			this.UpdateShips(rawData.api_ship.Select(id => this.homeport.Organization.Ships[id]).ToArray());
		}

		/// <summary>
		/// 艦隊の編成を変更します。
		/// </summary>
		/// <param name="index">編成を変更する艦のインデックス。通常は 0 ～ 5、旗艦以外をすべて外す場合は -1。</param>
		/// <param name="ship">艦隊の <paramref name="index"/> 番目に新たに編成する艦。<paramref name="index"/> 番目から艦を外す場合は null。</param>
		/// <returns>このメソッドを呼び出した時点で <paramref name="index"/> に配置されていた艦。</returns>
		internal Ship Change(int index, Ship ship)
		{
			var current = this.originalShips[index];

			List<Ship> list;
			if (index == -1)
			{
				list = this.originalShips.Take(1).ToList();
			}
			else
			{
				list = this.originalShips.ToList();
				list[index] = ship;
				list.RemoveAll(x => x == null);
			}

			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);

			return current;
		}


		/// <summary>
		/// 指定したインデックスの艦を艦隊から外します。
		/// </summary>
		/// <param name="index">艦隊から外す艦のインデックス。</param>
		internal void Unset(int index)
		{
			var list = this.originalShips.ToList();
			list[index] = null;
			list.RemoveAll(x => x == null);

			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);
		}

		/// <summary>
		/// 旗艦以外のすべての艦を艦隊から外します。
		/// </summary>
		internal void UnsetAll()
		{
			var list = this.originalShips.Take(1).ToList();
			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);
		}


		/// <summary>
		/// 艦隊の平均レベルや制空戦力などの各種数値を再計算します。
		/// </summary>
		internal void Calculate()
		{
			this.TotalLevel = this.Ships.HasItems() ? this.Ships.Sum(x => x.Level) : 0;
			this.AverageLevel = this.Ships.HasItems() ? (double)this.TotalLevel / this.Ships.Length : 0.0;
			this.AirSuperiorityPotential = this.Ships.Sum(s => s.CalcAirSuperiorityPotential());
			this.TotalViewRange = this.CalcFleetViewRange(KanColleClient.Current.Settings.ViewRangeCalcLogic);
			this.Speed = this.Ships.All(s => s.Info.Speed == Speed.Fast) ? Speed.Fast : Speed.Low;
		}


		internal void Sortie()
		{
			if (!this.isInSortie)
			{
				this.isInSortie = true;
				this.UpdateStatus();
			}
		}

		internal void Homing()
		{
			if (this.isInSortie)
			{
				this.isInSortie = false;
				this.UpdateStatus();
			}
		}


		/// <summary>
		/// 現在の艦隊情報を使用して、<see cref="State"/> プロパティを更新します。
		/// </summary>
		internal void UpdateStatus()
		{
			this.Condition.Update(this.Ships);
			this.IsWounded = this.Ships.Any(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25);
			this.IsRepairling = this.homeport.Repairyard.CheckRepairing(this);
			this.IsInShortSupply = this.Ships.Any(s => s.Fuel.Current < s.Fuel.Maximum || s.Bull.Current < s.Bull.Maximum);

			if (this.Ships.Length == 0)
			{
				this.State = FleetState.Empty;
			}
			else if (this.isInSortie)
			{
				this.State = FleetState.Sortie;
			}

			else if (this.Expedition.IsInExecution)
			{
				this.State = FleetState.Expedition;
			}
			else
			{

				this.State = FleetState.Homeport;
			}

			this.IsReady = this.State == FleetState.Homeport
						   && !this.Condition.IsRejuvenating
						   && !this.IsWounded
						   && !this.IsRepairling
						   && !this.IsInShortSupply;
		}


		private void UpdateShips(Ship[] ships)
		{
			this.originalShips = ships;
			this.Ships = ships.Where(x => x != null).ToArray();

			this.Calculate();
			this.UpdateStatus();
		}


		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Ships = {2}", this.Id, this.Name, this.Ships.Select(s => "\"" + s.Info.Name + "\"").ToString(","));
		}

		public virtual void Dispose()
		{
			this.compositeDisposable.Dispose();
			this.Expedition.SafeDispose();
			this.Condition.SafeDispose();
		}
	}
}
