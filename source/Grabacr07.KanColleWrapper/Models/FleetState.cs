using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatefulModel.EventListeners.WeakEvents;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦隊の状態を表します。
	/// </summary>
	public class FleetState : DisposableNotifier
	{
		private readonly Homeport homeport;
		private readonly Fleet[] source;

		/// <summary>
		/// 艦隊に編成されている艦娘のコンディションを取得します。
		/// </summary>
		public FleetCondition Condition { get; }

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

		private double _AirSuperiorityPotential;

		/// <summary>
		/// 艦隊の制空能力を取得します。
		/// </summary>
		public double AirSuperiorityPotential
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

		#region MinAirSuperiorityPotential 変更通知プロパティ

		private double _MinAirSuperiorityPotential;

		/// <summary>
		/// 艦隊の制空能力を取得します。
		/// </summary>
		public double MinAirSuperiorityPotential
		{
			get { return this._MinAirSuperiorityPotential; }
			private set
			{
				if (this._MinAirSuperiorityPotential != value)
				{
					this._MinAirSuperiorityPotential = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region MaxAirSuperiorityPotential 変更通知プロパティ

		private double _MaxAirSuperiorityPotential;

		/// <summary>
		/// 艦隊の制空能力を取得します。
		/// </summary>
		public double MaxAirSuperiorityPotential
		{
			get { return this._MaxAirSuperiorityPotential; }
			private set
			{
				if (this._MaxAirSuperiorityPotential != value)
				{
					this._MaxAirSuperiorityPotential = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ViewRange 変更通知プロパティ

		private double _ViewRange;

		/// <summary>
		/// 艦隊の索敵値を取得します。索敵の計算は <see cref="IKanColleClientSettings.ViewRangeCalcType"/> で指定された方法を使用します。
		/// </summary>
		public double ViewRange
		{
			get { return this._ViewRange; }
			private set
			{
				if (this._ViewRange != value)
				{
					this._ViewRange = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ViewRangeCalcType 変更通知プロパティ

		private string _ViewRangeCalcType;

		public string ViewRangeCalcType
		{
			get { return this._ViewRangeCalcType; }
			set
			{
				if (this._ViewRangeCalcType != value)
				{
					this._ViewRangeCalcType = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Speed 変更通知プロパティ

		private FleetSpeed _Speed;

		public FleetSpeed Speed
		{
			get { return this._Speed; }
			set
			{
				if (this._Speed != value)
				{
					this._Speed = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Situation 変更通知プロパティ

		private FleetSituation _Situation;

		public FleetSituation Situation
		{
			get { return this._Situation; }
			private set
			{
				if (this._Situation != value)
				{
					this._Situation = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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

		public event EventHandler Updated;
		public event EventHandler Calculated;


		public FleetState(Homeport homeport, params Fleet[] fleets)
		{
			this.homeport = homeport;
			this.source = fleets ?? new Fleet[0];

			this.Condition = new FleetCondition();
			this.CompositeDisposable.Add(this.Condition);
			this.CompositeDisposable.Add(new PropertyChangedWeakEventListener(KanColleClient.Current.Settings)
			{
				{ nameof(IKanColleClientSettings.ViewRangeCalcType), (_, __) => this.Calculate() },
				{ nameof(IKanColleClientSettings.IsViewRangeCalcIncludeFirstFleet), (_, __) => this.Calculate() },
				{ nameof(IKanColleClientSettings.IsViewRangeCalcIncludeSecondFleet), (_, __) => this.Calculate() },
			});
		}


		/// <summary>
		/// 艦隊の平均レベルや制空戦力などの各種数値を再計算します。
		/// </summary>
		public void Calculate()
		{
			var ships = this.source.SelectMany(x => x.Ships).WithoutEvacuated().ToArray();
			var firstFleetShips = this.source.FirstOrDefault()?.Ships.WithoutEvacuated().ToArray() ?? new Ship[0];

			this.TotalLevel = ships.HasItems() ? ships.Sum(x => x.Level) : 0;
			this.AverageLevel = ships.HasItems() ? (double)this.TotalLevel / ships.Length : 0.0;
			this.AirSuperiorityPotential = firstFleetShips.Sum(s => s.CalcAirSuperiorityPotential());
			this.MinAirSuperiorityPotential = firstFleetShips.Sum(s => s.CalcMinAirSuperiorityPotential());
			this.MaxAirSuperiorityPotential = firstFleetShips.Sum(s => s.CalcMaxAirSuperiorityPotential());
			this.Speed = ships.All(x => x.Info.Speed == ShipSpeed.Fast)
				? FleetSpeed.Fast
				: ships.All(x => x.Info.Speed == ShipSpeed.Low)
					? FleetSpeed.Low
					: FleetSpeed.Hybrid;

			var logic = ViewRangeCalcLogic.Get(KanColleClient.Current.Settings.ViewRangeCalcType);
			this.ViewRange = logic.Calc(this.source);
			this.ViewRangeCalcType = logic.Name;

			this.Calculated?.Invoke(this, new EventArgs());
		}

		public void Update()
		{
			var state = FleetSituation.Empty;
			var ready = true;

			var ships = this.source.SelectMany(x => x.Ships).ToArray();
			if (ships.Length == 0)
			{
				ready = false;
			}
			else
			{
				var first = this.source[0];

				if (this.source.Length == 1)
				{
					if (first.IsInSortie)
					{
						state |= FleetSituation.Sortie;
						ready = false;
					}
					else if (first.Expedition.IsInExecution)
					{
						state |= FleetSituation.Expedition;
						ready = false;
					}
					else
					{
						state |= FleetSituation.Homeport;
					}
				}
				else
				{
					state |= FleetSituation.Combined;

					if (first.IsInSortie)
					{
						state |= FleetSituation.Sortie;
						ready = false;
					}
					else
					{
						state |= FleetSituation.Homeport;
					}
				}
			}

			this.Condition.Update(ships);
			this.Condition.IsEnabled = state.HasFlag(FleetSituation.Homeport); // 疲労回復通知は母港待機中の艦隊でのみ行う

			if (state.HasFlag(FleetSituation.Homeport))
			{
				var repairing = ships.Any(x => this.homeport.Repairyard.CheckRepairing(x.Id));
				if (repairing)
				{
					state |= FleetSituation.Repairing;
					ready = false;
				}

				var inShortSupply = ships.Any(s => s.Fuel.Current < s.Fuel.Maximum || s.Bull.Current < s.Bull.Maximum);
				if (inShortSupply)
				{
					state |= FleetSituation.InShortSupply;
					ready = false;
				}

				if (this.Condition.IsRejuvenating)
				{
					ready = false;
				}
			}

			var heavilyDamaged = ships
				.Where(s => !this.homeport.Repairyard.CheckRepairing(s.Id))
				.Where(s => !s.Situation.HasFlag(ShipSituation.Evacuation) && !s.Situation.HasFlag(ShipSituation.Tow))
				.Where(s => !(state.HasFlag(FleetSituation.Sortie) && s.Situation.HasFlag(ShipSituation.DamageControlled)))
				.Any(s => s.HP.IsHeavilyDamage());
			if (heavilyDamaged)
			{
				state |= FleetSituation.HeavilyDamaged;
				ready = false;
			}

			var flagshipIsRepairShip = this.source
				.Where(x => x.Ships.Length >= 1)
				.Select(x => x.Ships[0])
				.Any(x => x.Info.ShipType.Id == 19);
			if (flagshipIsRepairShip)
			{
				state |= FleetSituation.FlagshipIsRepairShip;
				if (KanColleClient.Current.Settings.CheckFlagshipIsRepairShip) ready = false;
			}

			this.Situation = state;
			this.IsReady = ready;

			this.Updated?.Invoke(this, new EventArgs());
		}
	}
}
