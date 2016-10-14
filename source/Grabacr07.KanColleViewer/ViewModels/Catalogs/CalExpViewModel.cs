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
	public class CalExpViewModel : WindowViewModel
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
		public IEnumerable<string> SeaList { get; set; }
		public static Dictionary<string, int> SeaExpTable = new Dictionary<string, int>
		{
			{"1-1", 30}, {"1-2", 50}, {"1-3", 80}, {"1-4", 100}, {"1-5", 150},
			{"2-1", 120}, {"2-2", 150}, {"2-3", 200},{"2-4", 300},{"2-5", 250},
			{"3-1", 310}, {"3-2", 320}, {"3-3", 330}, {"3-4", 350},{"3-5",400},
			{"4-1", 310}, {"4-2", 320}, {"4-3", 330}, {"4-4", 340},
			{"5-1", 360}, {"5-2", 380}, {"5-3", 400}, {"5-4", 420}, {"5-5", 450},
			{"6-1", 380}, {"6-2", 420}
		};

		public IEnumerable<string> ResultList { get; set; }
		public string[] Results = { "S", "A", "B", "C", "D", "E" };

		private readonly Subject<Unit> updateSource = new Subject<Unit>();
		private readonly Homeport homeport = KanColleClient.Current.Homeport;

		public ShipCatalogSortWorker SortWorker { get; set; }

        #region TabItems 변경통지 프로퍼티

        private string[] _TabItems;
        public string[] TabItems
        {
            get { return this._TabItems; }
            set
            {
                this._TabItems = value;
                this.RaisePropertyChanged();
            }
        }

        private string _SelectedTab;
        public string SelectedTab
        {
            get { return this._SelectedTab; }
            set
            {
                this._SelectedTab = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("SelectedTabIdx");
                this.RaisePropertyChanged("GoalExpVisible");
                this.RaisePropertyChanged("TrainingExpVisible");
                this.UpdateExpCalculator();
            }
        }

        public int SelectedTabIdx => this.SelectedTab == null ? 0 : (this.TabItems?.ToList().IndexOf(this.SelectedTab) ?? 0);

        public bool GoalExpVisible => this.SelectedTabIdx == 0;
        public bool TrainingExpVisible => this.SelectedTabIdx == 1;

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
                this._Training_FlagshipExp = value;
                this.RaisePropertyChanged();
            }
        }
        public int Training_FlagshipMvpExp
        {
            get { return this._Training_FlagshipMvpExp; }
            set
            {
                this._Training_FlagshipMvpExp = value;
                this.RaisePropertyChanged();
            }
        }
        public int Training_AccshipExp
        {
            get { return this._Training_AccshipExp; }
            set
            {
                this._Training_AccshipExp = value;
                this.RaisePropertyChanged();
            }
        }
        public int Training_AccshipMvpExp
        {
            get { return this._Training_AccshipMvpExp; }
            set
            {
                this._Training_AccshipMvpExp = value;
                this.RaisePropertyChanged();
            }
        }
        public int Training_TrainingCruiser_Bonus
        {
            get { return this._Training_TrainingCruiser_Bonus; }
            set
            {
                this._Training_TrainingCruiser_Bonus = value;
                this.RaisePropertyChanged();
            }
        }

        public int Training_Flagship_Lv
        {
            get { return this._Training_Flagship_Lv; }
            set
            {
                this._Training_Flagship_Lv = value;
                this.RaisePropertyChanged();
                this.UpdateExpCalculator();
            }
        }
        public int Training_Secondship_Lv
        {
            get { return this._Training_Secondship_Lv; }
            set
            {
                this._Training_Secondship_Lv = value;
                this.RaisePropertyChanged();
                this.UpdateExpCalculator();
            }
        }
        public bool Training_Secondship
        {
            get { return this._Training_Secondship; }
            set
            {
                this._Training_Secondship = value;
                this.RaisePropertyChanged();
                this.UpdateExpCalculator();
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
                    this.UpdateExpCalculator();
                }
            }
        }

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
						this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
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
					this.UpdateExpCalculator();
				}
			}
		}

		#endregion

		#region IsReloading 変更通知プロパティ

		private bool _IsReloading;

		public bool IsReloading
		{
			get { return this._IsReloading; }
			set
			{
				if (this._IsReloading != value)
				{
					this._IsReloading = value;
					this.RaisePropertyChanged();
					this.UpdateExpCalculator();
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


		public CalExpViewModel()
		{
			this.Title = "경험치 계산기";
			this.SeaList = SeaExpTable.Keys.ToList();
			this.ResultList = Results.ToList();

			this.SortWorker = new ShipCatalogSortWorker();

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.Throttle(TimeSpan.FromMilliseconds(7.0))
				.Do(_ => this.UpdateCore())
				.Subscribe(_ => this.IsReloading = false);
			this.CompositeDisposable.Add(this.updateSource);

			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.homeport)
			{
				{ () => this.homeport.Organization.Ships, (sender, args) => this.Update() },
			});

			SelectedSea = SeaExpTable.Keys.FirstOrDefault();
			SelectedResult = Results.FirstOrDefault();
            SelectedExpResult = Results.FirstOrDefault();

            this.TabItems = new string[]
            {
                "목표 경험치",
                "연습전 경험치"
            };
            this.SelectedTab = this.TabItems.FirstOrDefault();

            this.Update();
		}

		public void Update()
		{
			this.RaisePropertyChanged("AllShipTypes");
			this.updateSource.OnNext(Unit.Default);
		}

		private void UpdateCore()
		{
			var list = this.homeport.Organization.Ships.Values;

			this.Ships = this.SortWorker.Sort(list)
				.Select((x, i) => new ShipViewModel(i + 1, x, null))
				.ToList();
		}

		/// <summary>
		/// Calculates experience given parameters. Requires levels and experience to work with.
		/// </summary>
		public void UpdateExpCalculator()
		{
            if (this.SelectedTabIdx == 0)
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
            else if (this.SelectedTabIdx == 1)
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
                        tRate = TrainingRate(tShip[0].Level, 0); // Flagship only
                    else if (tShip.Length == 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id != 21)
                        tRate = TrainingRate(tShip[0].Level, 1); // One at Accompanies
                    else if (tShip.Length > 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id == 21)
                        tRate = TrainingRate(FirstFleet.FirstOrDefault().Level, 2); // Flagship and Accompany
                    else if (tShip.Length > 1 && FirstFleet.FirstOrDefault().Info.ShipType.Id != 21)
                        tRate = TrainingRate(Math.Max(tShip[0].Level, tShip[1].Level), 3); // Two at Accompanies
                }
                tExp = (int)Math.Floor(tRate * baseExp / 100);

                this.Training_FlagshipExp = (int)(baseExp * flagshipRate);
                this.Training_FlagshipMvpExp = (int)(baseExp * flagshipRate * mvpRate);
                this.Training_AccshipExp = (int)(baseExp);
                this.Training_AccshipMvpExp = (int)(baseExp * mvpRate);
                this.Training_TrainingCruiser_Bonus = tExp;
            }
		}

        private double TrainingRate(int tLevel, int rateType)
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

	}
}
