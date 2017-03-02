using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.EventListeners;
using MetroTrilithon.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class CalculatorViewModel : WindowViewModel
	{
		/// <summary>
		/// Completely experience table from 1 to 155. Each line = 20 levels
		/// </summary>
		public static int[] ExpTable = new int[] { 0, 0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500, 5500, 6600, 7800, 9100, 10500, 12000, 13600, 15300, 17100, 19000,
			21000, 23100, 25300, 27600, 30000, 32500, 35100, 37800, 40600, 43500, 46500, 49600, 52800, 56100, 59500, 63000, 66600, 70300, 74100, 78000,
			82000, 86100, 90300, 94600, 99000, 103500, 108100, 112800, 117600, 122500, 127500, 132700, 138100, 143700, 149500, 155500, 161700, 168100, 174700, 181500,
			188500, 195800, 203400, 211300, 219500, 228000, 236800, 245900, 255300, 265000, 275000, 285400, 296200, 307400, 319000, 331000, 343400, 356200, 369400, 383000,
			397000, 411500, 426500, 442000, 458000, 474500, 491500, 509000, 527000, 545500, 564500, 584500, 606500, 631500, 661500, 701500, 761500, 851500, 1000000, 1000000,
			1010000, 1011000, 1013000, 1016000, 1020000, 1025000, 1031000, 1038000, 1046000, 1055000, 1065000, 1077000, 1091000, 1107000, 1125000, 1145000, 1168000, 1194000, 1223000, 1255000,
			1290000, 1329000, 1372000, 1419000, 1470000, 1525000, 1584000, 1647000, 1714000, 1785000, 1860000, 1940000, 2025000, 2115000, 2210000, 2310000, 2415000, 2525000, 2640000, 2760000,
			2887000, 3021000, 3162000, 3310000, 3465000, 3628000, 3799000, 3978000, 4165000, 4360000, 4564000, 4777000, 4999000, 5230000, 5470000 };

		/// <summary>
		/// Sea exp table. Cannot be used properly in xaml without dumb workarounds.
		/// </summary>
		public static Dictionary<string, int> SeaExpTable = new Dictionary<string, int>
		{
			{"1-1", 30}, {"1-2", 50}, {"1-3", 80}, {"1-4", 100}, {"1-5", 150},
			{"2-1", 120}, {"2-2", 150}, {"2-3", 200},{"2-4", 300},{"2-5", 250},
			{"3-1", 310}, {"3-2", 320}, {"3-3", 330}, {"3-4", 350},{"3-5",400},
			{"4-1", 310}, {"4-2", 320}, {"4-3", 330}, {"4-4", 340},
			{"5-1", 360}, {"5-2", 380}, {"5-3", 400}, {"5-4", 420}, {"5-5", 450},
			{"6-1", 380}, {"6-2", 420}
		};
		public IEnumerable<string> SeaList => CalculatorViewModel.SeaExpTable.Keys.ToList();

		public string[] ResultRanks { get; } = new string[] { "S", "A", "B", "C", "D", "E" };
		public IEnumerable<string> ResultList => this.ResultRanks.ToList();

		public string[] LandBasedType { get; } = new string[] { "출격", "방공" };
		public IEnumerable<string> LandBasedTypeList => this.LandBasedType.ToList();


		private readonly Subject<Unit> UpdateSourceShipList = new Subject<Unit>();
		private readonly Subject<Unit> UpdateSourceSlotitemList = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		#region TabItems 변경통지 프로퍼티
		private string[] _TabItems;
		public string[] TabItems
		{
			get { return this._TabItems; }
			set
			{
				if (this._TabItems != value)
				{
					this._TabItems = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string _SelectedTab;
		public string SelectedTab
		{
			get { return this._SelectedTab; }
			set
			{
				if (this._SelectedTab != value)
				{
					this._SelectedTab = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("SelectedTabIdx");
					this.UpdateCalculator();
				}
			}
		}

		public int SelectedTabIdx => this.SelectedTab == null
			? 0
			: (this.TabItems?.ToList().IndexOf(this.SelectedTab) ?? 0);
		#endregion

		#region Ships 変更通知プロパティ
		private IReadOnlyCollection<ShipViewModel> _Ships;
		public IReadOnlyCollection<ShipViewModel> Ships
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

		#region IsReloading 変更通知プロパティ
		private int _Reloading;
		public int Reloading
		{
			get { return this._Reloading; }
			set
			{
				if (this._Reloading != value)
				{
					this._Reloading = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(IsReloading));
				}
			}
		}

		public bool IsReloading => this.Reloading > 0;
		#endregion


		#region CurrentShip 変更通知プロパティ
		private Ship _CurrentShip;
		public Ship CurrentShip
		{
			get { return this._CurrentShip; }
			set
			{
				if (this._CurrentShip != value)
				{
					this._CurrentShip = value;
					if (value != null)
					{
						this.CurrentLevel = this.CurrentShip.Level;
						this.TargetLevel = Math.Min(this.CurrentShip.Level + 1, 155);
						if (this.CurrentShip.Info.NextRemodelingLevel.HasValue)
						{
							if (this.CurrentShip.Info.NextRemodelingLevel.Value > this.CurrentLevel)
							{
								this.RemodelLv = this.CurrentShip.Info.NextRemodelingLevel.Value;
								this.TargetLevel = RemodelLv;
							}
						}
						this.CurrentExp = this.CurrentShip.Exp;
						this.UpdateCalculator();
						this.RaisePropertyChanged();
					}
				}
			}
		}
		#endregion

		#region CurrentLevel 変更通知プロパティ
		private int _CurrentLevel;
		public int CurrentLevel
		{
			get { return this._CurrentLevel; }
			set
			{
				if (this._CurrentLevel != value && value >= 1 && value <= 155)
				{
					this._CurrentLevel = value;
					this.CurrentExp = ExpTable[value];
					this.TargetLevel = Math.Max(this.TargetLevel, Math.Min(value + 1, 155));
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region TargetLevel 変更通知プロパティ
		private int _TargetLevel;
		public int TargetLevel
		{
			get { return this._TargetLevel; }
			set
			{
				if (this._TargetLevel != value && value >= 1 && value <= 155)
				{
					this._TargetLevel = value;
					this.TargetExp = ExpTable[value];
					this.CurrentLevel = Math.Min(this.CurrentLevel, Math.Max(value - 1, 1));
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region SelectedSea 変更通知プロパティ
		private string _SelectedSea;
		public string SelectedSea
		{
			get { return this._SelectedSea; }
			set
			{
				if (_SelectedSea != value)
				{
					this._SelectedSea = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region SelectedResult 変更通知プロパティ
		private string _SelectedResult;
		public string SelectedResult
		{
			get { return this._SelectedResult; }
			set
			{
				if (this._SelectedResult != value)
				{
					this._SelectedResult = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region IsFlagship 変更通知プロパティ
		private bool _IsFlagship;
		public bool IsFlagship
		{
			get { return this._IsFlagship; }
			set
			{
				if (this._IsFlagship != value)
				{
					this._IsFlagship = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region IsMVP 変更通知プロパティ
		private bool _IsMVP;
		public bool IsMVP
		{
			get { return this._IsMVP; }
			set
			{
				if (this._IsMVP != value)
				{
					this._IsMVP = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region CurrentExp 変更通知プロパティ
		private int _CurrentExp;
		public int CurrentExp
		{
			get { return this._CurrentExp; }
			set
			{
				if (this._CurrentExp != value)
				{
					this._CurrentExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region TargetExp 変更通知プロパティ
		private int _TargetExp;
		public int TargetExp
		{
			get { return this._TargetExp; }
			set
			{
				if (this._TargetExp != value)
				{
					this._TargetExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RemodelLv 変更通知プロパティ
		private int _RemodelLv;
		public int RemodelLv
		{
			get { return this._RemodelLv; }
			set
			{
				if (this._RemodelLv != value)
				{
					this._RemodelLv = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SortieExp 変更通知プロパティ
		private int _SortieExp;
		public int SortieExp
		{
			get { return this._SortieExp; }
			set
			{
				if (this._SortieExp != value)
				{
					this._SortieExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RemainingExp 変更通知プロパティ
		private int _RemainingExp;
		public int RemainingExp
		{
			get { return this._RemainingExp; }
			set
			{
				if (this._RemainingExp != value)
				{
					this._RemainingExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RunCount 変更通知プロパティ
		private int _RunCount;
		public int RunCount
		{
			get { return this._RunCount; }
			set
			{
				if (this._RunCount != value)
				{
					this._RunCount = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		#region 연습전 경험치 프로퍼티
		private int _Training_FlagshipExp;
		private int _Training_FlagshipMvpExp;
		private int _Training_AccshipExp;
		private int _Training_AccshipMvpExp;
		private int _Training_TrainingCruiser_Bonus;

		private int _Training_Flagship_Lv;
		private int _Training_Secondship_Lv;
		private bool _Training_Secondship;

		public int Training_FlagshipExp
		{
			get { return this._Training_FlagshipExp; }
			set
			{
				if (this._Training_FlagshipExp != value)
				{
					this._Training_FlagshipExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int Training_FlagshipMvpExp
		{
			get { return this._Training_FlagshipMvpExp; }
			set
			{
				if (this._Training_FlagshipMvpExp != value)
				{
					this._Training_FlagshipMvpExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int Training_AccshipExp
		{
			get { return this._Training_AccshipExp; }
			set
			{
				if (this._Training_AccshipExp != value)
				{
					this._Training_AccshipExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int Training_AccshipMvpExp
		{
			get { return this._Training_AccshipMvpExp; }
			set
			{
				if (this._Training_AccshipMvpExp != value)
				{
					this._Training_AccshipMvpExp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public int Training_TrainingCruiser_Bonus
		{
			get { return this._Training_TrainingCruiser_Bonus; }
			set
			{
				if (this._Training_TrainingCruiser_Bonus != value)
				{
					this._Training_TrainingCruiser_Bonus = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public int Training_Flagship_Lv
		{
			get { return this._Training_Flagship_Lv; }
			set
			{
				if (this._Training_Flagship_Lv != value)
				{
					this._Training_Flagship_Lv = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		public int Training_Secondship_Lv
		{
			get { return this._Training_Secondship_Lv; }
			set
			{
				if (this._Training_Secondship_Lv != value)
				{
					this._Training_Secondship_Lv = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		public bool Training_Secondship
		{
			get { return this._Training_Secondship; }
			set
			{
				if (this._Training_Secondship != value)
				{
					this._Training_Secondship = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}

		private string _SelectedExpResult;
		public string SelectedExpResult
		{
			get { return this._SelectedExpResult; }
			set
			{
				if (this._SelectedExpResult != value)
				{
					this._SelectedExpResult = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion


		#region LandBased_Slots 변경통지 프로퍼티
		public ICollection<SlotItemViewModel> _LandBased_Slots;
		public ICollection<SlotItemViewModel> LandBased_Slots
		{
			get { return this._LandBased_Slots; }
			private set
			{
				if (this._LandBased_Slots != value)
				{
					this._LandBased_Slots = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SelectedLandBasedType 変更通知プロパティ
		private string _SelectedLandBasedType;
		public string SelectedLandBasedType
		{
			get { return this._SelectedLandBasedType; }
			set
			{
				if (this._SelectedLandBasedType != value)
				{
					this._SelectedLandBasedType = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion

		#region LandBased_AirSuperiorityPotential 変更通知プロパティ
		private double _LandBased_AirSuperiorityPotential;
		public double LandBased_AirSuperiorityPotential
		{
			get { return this._LandBased_AirSuperiorityPotential; }
			set
			{
				if (this._LandBased_AirSuperiorityPotential != value)
				{
					this._LandBased_AirSuperiorityPotential = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(LandBased_AirSuperiorityPotentialText));
				}
			}
		}

		public string LandBased_AirSuperiorityPotentialText => this.LandBased_AirSuperiorityPotential.ToString("0.#");
		#endregion
		#region LandBased_AttackPower 変更通知プロパティ
		private double _LandBased_AttackPower;
		public double LandBased_AttackPower
		{
			get { return this._LandBased_AttackPower; }
			set
			{
				if (this._LandBased_AttackPower != value)
				{
					this._LandBased_AttackPower = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(LandBased_AttackPowerText));
				}
			}
		}

		public string LandBased_AttackPowerText => this.LandBased_AttackPower.ToString("0.#");
		#endregion
		#region LandBased_Distance 変更通知プロパティ
		private int _LandBased_Distance;
		public int LandBased_Distance
		{
			get { return this._LandBased_Distance; }
			set
			{
				if (this._LandBased_Distance != value)
				{
					this._LandBased_Distance = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region LandBased_Slot1 変更通知プロパティ
		private SlotItemViewModel _LandBased_Slot1;
		public SlotItemViewModel LandBased_Slot1
		{
			get { return this._LandBased_Slot1; }
			set
			{
				if (this._LandBased_Slot1 != value)
				{
					this._LandBased_Slot1 = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion
		#region LandBased_Slot2 変更通知プロパティ
		private SlotItemViewModel _LandBased_Slot2;
		public SlotItemViewModel LandBased_Slot2
		{
			get { return this._LandBased_Slot2; }
			set
			{
				if (this._LandBased_Slot2 != value)
				{
					this._LandBased_Slot2 = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion
		#region LandBased_Slot3 変更通知プロパティ
		private SlotItemViewModel _LandBased_Slot3;
		public SlotItemViewModel LandBased_Slot3
		{
			get { return this._LandBased_Slot3; }
			set
			{
				if (this._LandBased_Slot3 != value)
				{
					this._LandBased_Slot3 = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion
		#region LandBased_Slot4 変更通知プロパティ
		private SlotItemViewModel _LandBased_Slot4;
		public SlotItemViewModel LandBased_Slot4
		{
			get { return this._LandBased_Slot4; }
			set
			{
				if (this._LandBased_Slot4 != value)
				{
					this._LandBased_Slot4 = value;
					this.RaisePropertyChanged();
					this.UpdateCalculator();
				}
			}
		}
		#endregion


		public CalculatorViewModel()
		{
			this.Title = "수치 계산기";
			this.TabItems = new string[]
			{
				"목표 경험치",
				"연습전 경험치",
				"기항대 계산기"
			};
			this.SelectedTab = this.TabItems.FirstOrDefault();

			#region Update Sources
			this.UpdateSourceShipList
				.Do(_ => this.Reloading++)
				.Throttle(TimeSpan.FromMilliseconds(7.0))
				.Do(_ => this.UpdateShipList())
				.Do(_ => this.Reloading--)
				.Subscribe(_ => this.UpdateCalculator());
			this.CompositeDisposable.Add(this.UpdateSourceShipList);

			this.UpdateSourceSlotitemList
				.Do(_ => this.Reloading++)
				.Throttle(TimeSpan.FromMilliseconds(7.0))
				.Do(_ => this.UpdateSlotItemList())
				.Do(_ => this.Reloading--)
				.Subscribe(_ => this.UpdateCalculator());
			this.CompositeDisposable.Add(this.UpdateSourceShipList);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport)
			{
				{ () => this.homeport.Organization, (_, __) => {
					this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Organization)
					{
						{ () => this.homeport.Organization.Ships, (sender, args) => this.RequestUpdateShipList() },
					});
				} },
				{ () => this.homeport.Itemyard, (_, __) => {
					this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport.Itemyard)
					{
						{ () => this.homeport.Itemyard.SlotItems, (sender, args) => this.RequestUpdateSlotitemList() },
					});
				} },
			});
			#endregion

			SelectedSea = SeaExpTable.Keys.FirstOrDefault();
			SelectedResult = ResultRanks.FirstOrDefault();
			SelectedExpResult = ResultRanks.FirstOrDefault();

			SelectedLandBasedType = LandBasedType.FirstOrDefault();

			this.RequestUpdateShipList();
			this.RequestUpdateSlotitemList();
		}

		public void RequestUpdateShipList() => this.UpdateSourceShipList.OnNext(Unit.Default);
		public void RequestUpdateSlotitemList() => this.UpdateSourceSlotitemList.OnNext(Unit.Default);

		/// <summary>
		/// Update ship list for calculator
		/// </summary>
		private void UpdateShipList()
		{
			var list = this.homeport.Organization.Ships.Values;

			this.Ships = list.OrderByDescending(x => x.Exp)
				.ThenBy(x => x.Id)
				.Select((x, i) => new ShipViewModel(i + 1, x, null))
				.ToList();
		}

		/// <summary>
		/// Update slotitem list for calculator
		/// </summary>
		private void UpdateSlotItemList()
		{
			var list = this.homeport.Itemyard.SlotItems.Values;

			var items = new List<SlotItemViewModel>();
			items.Add(new SlotItemViewModel(0, null));
			items.AddRange(
				list.Where(x => x.Info.IsNumerable)
					.OrderBy(x => x.Info.IconType)
					.ThenBy(x => x.Info.Id)
					.Distinct(x => x.NameWithLevel)
					.Select((x, i) => new SlotItemViewModel(x.Id, x))
			);
			this.LandBased_Slots = items;

			if (this.LandBased_Slot1 == null || !this.LandBased_Slots.Contains(this.LandBased_Slot1))
				this.LandBased_Slot1 = this.LandBased_Slots.FirstOrDefault();
			if (this.LandBased_Slot2 == null || !this.LandBased_Slots.Contains(this.LandBased_Slot1))
				this.LandBased_Slot2 = this.LandBased_Slots.FirstOrDefault();
			if (this.LandBased_Slot3 == null || !this.LandBased_Slots.Contains(this.LandBased_Slot1))
				this.LandBased_Slot3 = this.LandBased_Slots.FirstOrDefault();
			if (this.LandBased_Slot4 == null || !this.LandBased_Slots.Contains(this.LandBased_Slot1))
				this.LandBased_Slot4 = this.LandBased_Slots.FirstOrDefault();
		}


		/// <summary>
		/// Update Calculator Display Values
		/// </summary>
		public void UpdateCalculator()
		{
			switch (this.SelectedTabIdx)
			{
				case 0:
					CalculateRemainingExp();
					break;

				case 1:
					CalculateTrainingExp();
					break;

				case 2:
					CalculateLandBased();
					break;
			}
		}

		/// <summary>
		/// Calculates experience with given parameters.
		/// Requires levels and experience to work with.
		/// </summary>
		private void CalculateRemainingExp()
		{
			if (this.TargetLevel < this.CurrentLevel || this.TargetExp < this.CurrentExp ||
				this.SelectedResult == null || this.SelectedSea == null)
				return;

			var RankTable = new Dictionary<string, double>
				{
					{"S", 1.2 },
					{"A", 1.0 },
					{"B", 1.0 },
					{"C", 0.8 },
					{"D", 0.7 },
					{"E", 0.5 }
				};

			// Lawl at that this inline conditional.
			double Multiplier = (this.IsFlagship ? 1.5 : 1) * (this.IsMVP ? 2 : 1) * RankTable[this.SelectedResult];

			this.SortieExp = (int)Math.Round(SeaExpTable[this.SelectedSea] * Multiplier, 0, MidpointRounding.AwayFromZero);
			this.RemainingExp = this.TargetExp - this.CurrentExp;
			this.RunCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(this.RemainingExp) / Convert.ToDecimal(this.SortieExp)));
		}

		/// <summary>
		/// Calculates expected exp for training with given parameters.
		/// </summary>
		private void CalculateTrainingExp()
		{
			if (this.SelectedExpResult == null)
				return;

			var RankTable = new Dictionary<string, double>
				{
					{"S", 1.2 },
					{"A", 1.0 },
					{"B", 1.0 },
					{"C", 0.64 },
					{"D", 0.56 },
					{"E", 0.4 }
				};
			var rankRate = RankTable[this.SelectedExpResult];
			var flagshipRate = 1.5;
			var mvpRate = 2.0;
			var tRate = 0.0;
			var tExp = 0;

			int baseExp = ExpTable[this.Training_Flagship_Lv] / 100;
			if (this.Training_Secondship) baseExp += ExpTable[this.Training_Secondship_Lv] / 300;

			if (baseExp > 500) baseExp = 500 + (int)Math.Floor(Math.Sqrt(baseExp - 500));
			baseExp = (int)(baseExp * rankRate);

			var FirstFleet = this.homeport.Organization.Fleets.FirstOrDefault().Value.Ships;
			var tShip = FirstFleet.Where(x => x.Info.ShipType.Id == 21).ToArray(); // 21 is Training Cruiser

			if (tShip.Length > 0)
			{
				if (tShip.Length == 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id == 21)
					tRate = GetTrainingBonus(tShip[0].Level, 0); // Flagship only
				else if (tShip.Length == 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id != 21)
					tRate = GetTrainingBonus(tShip[0].Level, 1); // One at Accompanies
				else if (tShip.Length > 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id == 21)
					tRate = GetTrainingBonus(FirstFleet.FirstOrDefault().Level, 2); // Flagship and Accompany
				else if (tShip.Length > 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id != 21)
					tRate = GetTrainingBonus(Math.Max(tShip[0].Level, tShip[1].Level), 3); // Two at Accompanies
			}
			tExp = (int)Math.Floor(tRate * baseExp / 100);

			this.Training_FlagshipExp = (int)(baseExp * flagshipRate);
			this.Training_FlagshipMvpExp = (int)(baseExp * flagshipRate * mvpRate);
			this.Training_AccshipExp = (int)(baseExp);
			this.Training_AccshipMvpExp = (int)(baseExp * mvpRate);
			this.Training_TrainingCruiser_Bonus = tExp;
		}

		/// <summary>
		/// Calculates training ship's bonus exp rate
		/// </summary>
		private double GetTrainingBonus(int tLevel, int rateType)
		{
			var rateIdx = 0;
			if (tLevel <= 9) rateIdx = 0;
			else if (tLevel <= 29) rateIdx = 1;
			else if (tLevel <= 59) rateIdx = 2;
			else if (tLevel <= 99) rateIdx = 3;
			else rateIdx = 4;

			double[][] rateTable = new double[][]
			{
				new double[] {5,  8,  12, 15, 20},  // Flagship only
				new double[] {3,  5,  7,  10, 15},  // One at Accompanies
				new double[] {10, 13, 16, 20, 25},  // Flagship and Accompany
				new double[] {4,  6,  8,  12, 17.5} // Two at Accompanies
			};
			return rateTable[rateType][rateIdx];
		}

		/// <summary>
		/// Calculates land-base aerial support's air superiority potential
		/// </summary>
		private void CalculateLandBased()
		{
			Dictionary<int, int[]> distanceBonus = new Dictionary<int, int[]>()
			{
				{ 138, new int[] { 3, 3, 3, 3, 3, 3, 3, 3 } },
				{ 178, new int[] { 3, 3, 2, 2, 2, 2, 1, 1 } },
				{ 151, new int[] { 2, 2, 2, 2, 1, 1, 0, 0 } },
				{  54, new int[] { 2, 2, 2, 2, 1, 1, 0, 0 } },
				{  25, new int[] { 2, 2, 2, 1, 1, 0, 0, 0 } },
				{  61, new int[] { 2, 1, 1, 0, 0, 0, 0, 0 } },
			};
			Dictionary<int, Proficiency> proficiencies = new Dictionary<int, Proficiency>()
			{
				{ 0, new Proficiency(  0,   9,  0,  0) },
				{ 1, new Proficiency( 10,  24,  0,  1) },
				{ 2, new Proficiency( 25,  39,  2,  2) },
				{ 3, new Proficiency( 40,  54,  5,  3) },
				{ 4, new Proficiency( 55,  69,  9,  4) },
				{ 5, new Proficiency( 70,  84, 14,  5) },
				{ 6, new Proficiency( 85,  99, 14,  7) },
				{ 7, new Proficiency(100, 120, 22,  9) },
			};
			var def = AirSuperiorityCalculationOptions.Default;

			var items = new SlotItemViewModel[]
				{
					this.LandBased_Slot1,
					this.LandBased_Slot2,
					this.LandBased_Slot3,
					this.LandBased_Slot4
				}
				.Where(x => x.Display != null)
				.Select(x => x.Display);


			LandBased_AttackPower = 0;
			LandBased_AirSuperiorityPotential = 0;
			LandBased_Distance = 0;
			if (items.Count() == 0) return;

			#region Attack Power calculating
			var attackers = new SlotItemType[]
			{
				SlotItemType.艦上攻撃機,
				SlotItemType.艦上爆撃機,
				SlotItemType.噴式攻撃機,
				SlotItemType.噴式戦闘爆撃機,
				SlotItemType.陸上攻撃機,
				SlotItemType.水上爆撃機
			};
			var power_sum = items
				.Where(x => attackers.Contains(x.Info.Type))
				.Sum(item =>
				{
					var proficiency = proficiencies[item.Proficiency];
					double damage = 0;

					if (item.Info.Type == SlotItemType.陸上攻撃機)
					{
						damage = (item.Info.Torpedo + item.Info.Bomb) / 2; // P
						damage *= Math.Sqrt(1.8 * 18); // root 1.8N
						damage += 25;
						damage = Math.Floor(damage * 0.8);
						// Critical modifier skip
						// Contact multiplier skip
					}
					else
					{
						damage = (item.Info.Torpedo + item.Info.Bomb) / 2; // P
						damage *= Math.Sqrt(1.8 * 18); // root 1.8N
						damage += 25;
						damage = Math.Floor(damage * 0.8);
						// Critical modifier skip
						// Contact multiplier skip
						damage *= 1.8;
					}
					return damage;
				});
			#endregion

			#region Bonus rate calculate when Air Defence Mode
			var bonusRate = 1.0;
			if (this.SelectedLandBasedType == "방공")
			{
				if (items.Any(x => x.Info.Type == SlotItemType.艦上偵察機))
				{
					var viewrange = items
						.Where(x => x.Info.Type == SlotItemType.艦上偵察機)
						.Max(x => x.Info.ViewRange);

					if (viewrange <= 7)
						bonusRate = 1.2;
					else if (viewrange == 8)
						bonusRate = 1.25; // Maybe?
					else
						bonusRate = 1.3;
				}
				else if (items.Any(x => x.Info.Type == SlotItemType.水上偵察機))
				{
					var viewrange = items
						.Where(x => x.Info.Type == SlotItemType.水上偵察機)
						.Max(x => x.Info.ViewRange);

					if (viewrange <= 7)
						bonusRate = 1.1;
					else if (viewrange == 8)
						bonusRate = 1.13;
					else
						bonusRate = 1.16;
				}
			}
			#endregion
			#region AA calculating
			var air_sum = items.Sum(item =>
				{
					var proficiency = proficiencies[item.Proficiency];
					double aa = item.Info.AA;
					double bonus = 0;

					switch (item.Info.Type)
					{
						// 전투기
						case SlotItemType.艦上戦闘機:
						case SlotItemType.水上戦闘機:
						case SlotItemType.噴式戦闘機:
						case SlotItemType.局地戦闘機:
							aa += item.Level * 0.2;
							bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
								+ proficiency.FighterBonus;
							break;

						// 공격기 (뇌격기, 폭격기)
						case SlotItemType.艦上攻撃機:
						case SlotItemType.艦上爆撃機:
						case SlotItemType.噴式攻撃機:
						case SlotItemType.噴式戦闘爆撃機:
						case SlotItemType.陸上攻撃機:
							bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
								+ 0;
							break;

						// 수상폭격기
						case SlotItemType.水上爆撃機:
							bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
								+ proficiency.SeaplaneBomberBonus;
							break;

						// 정찰기, 수상정찰기, (분식정찰기?)
						// 본래는 제공치에 포함되지 않으나 기항대에는 포함되나?
						// 다만 어차피 대공이 안붙어있음
						default:
							break;
					}
					bonus = Math.Min(22, bonus) + Math.Sqrt(12);

					switch (this.SelectedLandBasedType)
					{
						case "출격":
							if (item.Info.Type == SlotItemType.局地戦闘機)
								aa += (int)(item.Info.Evade * 1.5);
							break;
						case "방공":
							if (item.Info.Type == SlotItemType.局地戦闘機)
								aa += item.Info.Hit * 2 +item.Info.Evade;
							break;
					}
					return Math.Floor(Math.Sqrt(18) * aa) * bonusRate + Math.Floor(bonus);
				});
			#endregion

			int distance = items.Min(x => x.Info.Distance);
			#region Bonus Distance
			if (items.Any(x => distanceBonus.ContainsKey(x.Info.Id)))
			{
				var dist = Math.Max(0, Math.Min(7, distance - 2));
				distance += items
					.Where(x => distanceBonus.ContainsKey(x.Info.Id))
					.Max(x => distanceBonus[x.Info.Id][dist]);
			}
			#endregion

			LandBased_AttackPower = power_sum;
			LandBased_AirSuperiorityPotential = air_sum;
			LandBased_Distance = distance;
		}

		private class Proficiency
		{
			private int internalMinValue { get; }
			private int internalMaxValue { get; }

			public int FighterBonus { get; }
			public int SeaplaneBomberBonus { get; }

			public Proficiency(int internalMin, int internalMax, int fighterBonus, int seaplaneBomberBonus)
			{
				this.internalMinValue = internalMin;
				this.internalMaxValue = internalMax;
				this.FighterBonus = fighterBonus;
				this.SeaplaneBomberBonus = seaplaneBomberBonus;
			}

			/// <summary>
			/// 内部熟練度値を取得します。
			/// </summary>
			public int GetInternalValue(AirSuperiorityCalculationOptions options)
			{
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMinValue)) return this.internalMinValue;
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMaxValue)) return this.internalMaxValue;
				return (this.internalMaxValue + this.internalMinValue) / 2; // <- めっちゃ適当
			}
		}
	}

	public class SlotItemViewModel : Livet.ViewModel
	{
		public int Key { get; }
		public SlotItem Display { get; }

		public SlotItemViewModel(int Key, SlotItem Display)
		{
			this.Key = Key;
			this.Display = Display;
		}
	}
}
