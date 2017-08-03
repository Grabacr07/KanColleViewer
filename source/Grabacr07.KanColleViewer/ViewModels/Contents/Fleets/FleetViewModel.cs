﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Grabacr07.KanColleWrapper;
using System.Windows;
using System.Text;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 単一の艦隊情報を提供します。
	/// </summary>
	public class FleetViewModel : ItemViewModel
	{
		public Fleet Source { get; }

		public int Id => this.Source.Id;

		public string Name => string.IsNullOrEmpty(this.Source.Name.Trim())
			? "(제 " + this.Source.Id + " 함대)"
			: this.Source.Name;

		#region string
		private string _ShipTypeString;
		public string ShipTypeString
		{
			get { return this._ShipTypeString; }
			set
			{
				if (this._ShipTypeString == value) return;
				this._ShipTypeString = value;
				this.RaisePropertyChanged();
			}
		}
		private string _FlagLv;
		public string FlagLv
		{
			get { return this._FlagLv; }
			set
			{
				if (this._FlagLv == value) return;
				this._FlagLv = value;
				this.RaisePropertyChanged();
			}
		}
		private string _FlagType;
		public string FlagType
		{
			get { return this._FlagType; }
			set
			{
				if (this._FlagType == value) return;
				this._FlagType = value;
				this.RaisePropertyChanged();
			}
		}
		private string _TotalLv;
		public string TotalLv
		{
			get { return this._TotalLv; }
			set
			{
				if (this._TotalLv == value) return;
				this._TotalLv = value;
				this.RaisePropertyChanged();
			}
		}
		private string _nDrum;
		public string nDrum
		{
			get { return this._nDrum; }
			set
			{
				if (this._nDrum == value) return;
				this._nDrum = value;
				this.RaisePropertyChanged();
			}
		}
		private int _nFuelLoss;
		public int nFuelLoss
		{
			get { return this._nFuelLoss; }
			set
			{
				if (this._nFuelLoss == value) return;
				this._nFuelLoss = value;
				this.RaisePropertyChanged();
			}
		}
		private int _nArmoLoss;
		public int nArmoLoss
		{
			get { return this._nArmoLoss; }
			set
			{
				if (this._nArmoLoss == value) return;
				this._nArmoLoss = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region 원정 구성 확인
		private bool _IsPassed;
		public bool IsPassed
		{
			get { return this._IsPassed; }
			set
			{
				if (this._IsPassed == value) return;
				this._IsPassed = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region 선택된 원정번호 (원정 탭 수정)

		private ExpeditionResultData _ResultData { get; set; }
		public ExpeditionResultData ResultData
		{
			get { return this._ResultData; }
			set
			{
				if (this._ResultData != value)
				{
					this._ResultData = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private int _ExpeditionId;
		public int ExpeditionId
		{
			get { return this._ExpeditionId; }
			set
			{
				// if (this._ExpeditionId == value) return;
				// 같은 값을 넣는 것으로 원정 성공 여부 등을 재계산해야함

				this._ExpeditionId = value;
				this.IsPassed = this.CompareExpeditionData(value, this.Source.Ships);

				this.IsPossible = !this.IsPassed
					? ExpeditionPossible.NotAccepted
					: this.Source.Ships.Where(x => (x.Fuel.Current != x.Fuel.Maximum) || x.Bull.Current != x.Bull.Maximum).Count() > 0
						? ExpeditionPossible.NotSupplied
						: ExpeditionPossible.Possible;
				this.GreatChance = this.IsPossible == ExpeditionPossible.Possible
					? (int)ExpeditionExtension.CheckGreateSuccessChance(value, this.Source.Ships.ToArray())
					: 0;

				this.RaisePropertyChanged();

				this.ResultData = new ExpeditionResultData(value, this.Ships);
			}
		}

		#endregion

		#region Visibility

		private Visibility _IsFirstFleet;
		public Visibility IsFirstFleet
		{
			get { return this._IsFirstFleet; }
			set
			{
				if (this._IsFirstFleet == value) return;
				this._IsFirstFleet = value;
				this.RaisePropertyChanged();
			}
		}

		private Visibility _vFlag;
		public Visibility vFlag
		{
			get { return this._vFlag; }
			set
			{
				if (this._vFlag == value) return;
				this._vFlag = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vTotal;
		public Visibility vTotal
		{
			get { return this._vTotal; }
			set
			{
				if (this._vTotal == value) return;
				this._vTotal = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vFlagType;
		public Visibility vFlagType
		{
			get { return this._vFlagType; }
			set
			{
				if (this._vFlagType == value) return;
				this._vFlagType = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vNeed;
		public Visibility vNeed
		{
			get { return this._vNeed; }
			set
			{
				if (this._vNeed == value) return;
				this._vNeed = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vDrum;
		public Visibility vDrum
		{
			get { return this._vDrum; }
			set
			{
				if (this._vDrum == value) return;
				this._vDrum = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vResource;
		public Visibility vResource
		{
			get { return this._vResource; }
			set
			{
				if (this._vResource == value) return;
				this._vResource = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vArmo;
		public Visibility vArmo
		{
			get { return this._vArmo; }
			set
			{
				if (this._vArmo == value) return;
				this._vArmo = value;
				this.RaisePropertyChanged();
			}
		}
		private Visibility _vFuel;
		public Visibility vFuel
		{
			get { return this._vFuel; }
			set
			{
				if (this._vFuel == value) return;
				this._vFuel = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion


		#region 원정 탭 수정

		public Dictionary<int, string> ExpeditionList => ExpeditionExtension.ExpeditionList;

		// 편성 성공 여부
		private ExpeditionPossible _IsPossible;
		public ExpeditionPossible IsPossible
		{
			get { return this._IsPossible; }
			set
			{
				this._IsPossible = value;
				this.RaisePropertyChanged();
			}
		}

		// 출격 여부
		public bool IsInSortie => (this.Source.State.Situation & FleetSituation.Sortie) == FleetSituation.Sortie;

		// 대성공 확률
		private int _GreatChance;
		public int GreatChance
		{
			get { return this._GreatChance; }
			set
			{
				this._GreatChance = value;
				this.RaisePropertyChanged();
				this.RaisePropertyChanged(nameof(this.CanGreatSuccess));
				this.RaisePropertyChanged(nameof(this.GreatChanceText));
			}
		}
		public bool CanGreatSuccess => this.GreatChance > 0;
		public string GreatChanceText => string.Format("대성공 {0}%", this.GreatChance);

		#endregion

		#region 보급량

		public int UsedFuel => this.Ships.Sum(x => x.Ship.UsedFuel);
		public int UsedAmmo => this.Ships.Sum(x => x.Ship.UsedBull);
		public int UsedBauxite => this.Ships.Sum(x => x.Ship.Slots.Sum(y => y.Lost * 5));

		#endregion

		/// <summary>
		/// 艦隊に所属している艦娘のコレクションを取得します。
		/// </summary>
		public ShipViewModel[] Ships
		{
			get
			{
				this.ShipTypeTable = MakeShipTypeTable(this.Source.Ships);
				this.IsPassed = CompareExpeditionData(this.ExpeditionId, this.Source.Ships);

				return this.Source.Ships.Select(x => new ShipViewModel(x)).ToArray();
			}
		}

		public List<int> ResultList { get; set; }
		private Dictionary<int, int> ShipTypeTable { get; set; }

		public FleetStateViewModel State { get; }

		public ExpeditionViewModel Expedition { get; }

		public ViewModel QuickStateView
		{
			get
			{
				var situation = this.Source.State.Situation;
				if (situation == FleetSituation.Empty)
				{
					return NullViewModel.Instance;
				}
				if (situation.HasFlag(FleetSituation.Sortie))
				{
					return this.State.Sortie;
				}
				if (situation.HasFlag(FleetSituation.Expedition))
				{
					return this.Expedition;
				}

				return this.State.Homeport;
			}
		}


		public FleetViewModel(Fleet fleet)
		{
			this.Source = fleet;
			this.ShipTypeTable = MakeShipTypeTable(this.Source.Ships);

			if (this.Source.Id > 1)
			{
				IsFirstFleet = Visibility.Visible;

				this.ResultList = this.MakeResultList();
				this.ExpeditionId = fleet.Expedition.IsInExecution
					? fleet.Expedition.Id
					: this.ResultList.FirstOrDefault();
			}
			else IsFirstFleet = Visibility.Collapsed;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
				{ nameof(fleet.ShipsUpdated), (sender,args) => this.ExpeditionId = this.ExpeditionId }
			});

			fleet.State.Updated += (sender, args) => this.ExpeditionId = this.ExpeditionId; // 편성 변경?

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ nameof(fleet.State.Situation), (sender, args) => this.RaisePropertyChanged(nameof(this.QuickStateView)) },
			});

			this.State = new FleetStateViewModel(fleet.State);
			this.CompositeDisposable.Add(this.State);

			this.Expedition = new ExpeditionViewModel(fleet.Expedition);
			this.CompositeDisposable.Add(this.Expedition);

			// 원정중 여부 변경
			this.CompositeDisposable.Add(new PropertyChangedEventListener(this.Expedition)
			{
				{nameof(this.Expedition.IsInExecution), (sender,args) => this.ExpeditionId = (this.Expedition.IsInExecution ? this.Expedition.Mission.Id : this.ExpeditionId) }
			});
			// 연료/탄약량 변경
			// 출격 등 상태 변경
			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ nameof(fleet.State.Situation), (sender, args) => this.RaisePropertyChanged(nameof(this.IsInSortie)) },
				{  nameof(fleet.State.UsedFuel), (sender,args) =>
					{
						this.RaisePropertyChanged(nameof(this.UsedFuel));
						this.RaisePropertyChanged(nameof(this.UsedAmmo));
						this.RaisePropertyChanged(nameof(this.UsedBauxite));
					}
				},
				{  nameof(fleet.State.UsedBull), (sender,args) =>
					{
						this.RaisePropertyChanged(nameof(this.UsedFuel));
						this.RaisePropertyChanged(nameof(this.UsedAmmo));
						this.RaisePropertyChanged(nameof(this.UsedBauxite));
					}
				}
			});
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="total"></param>
		/// <param name="MissonNum"></param>
		/// <param name="ResourceType">0=연료 1=탄</param>
		/// <returns></returns>
		private int LossResource(Ship[] fleet, int MissionNum, int ResourceType = 0)
		{
			double total = 0;

			string losstype = "FuelLoss";
			if (ResourceType == 1) losstype = "ArmoLoss";

			var percent = KanColleClient.Current.Translations.GetExpeditionData(losstype, MissionNum);
			if (string.IsNullOrEmpty(percent)) return -1;

			double LossPercent = (double)Convert.ToInt32(percent) / 100d;

			total = fleet.Sum(x =>
			{
				if (ResourceType == 1)
					return (int)Math.Floor((double)x.Bull.Maximum * LossPercent);
				else
					return (int)Math.Floor((double)x.Fuel.Maximum * LossPercent);
			});

			if (ResourceType == 1)
				this.vArmo = Visibility.Visible;
			else
				this.vFuel = Visibility.Visible;

			this.vResource = Visibility.Visible;
			return (int)total;
		}
		private bool DrumCount(int Mission, Ship[] fleet)
		{
			bool result = false;
			var NeedDrumRaw = KanColleClient.Current.Translations.GetExpeditionData("DrumCount", Mission);

			var sNeedDrumRaw = NeedDrumRaw.Split(';');
			int nTotalDrum = Convert.ToInt32(sNeedDrumRaw[0]);
			int nHasDrumShip = Convert.ToInt32(sNeedDrumRaw[1]);
			this.nDrum = "총" + sNeedDrumRaw[0] + "개, " + "장착칸무스 최소 " + sNeedDrumRaw[1] + "척";
			this.vDrum = Visibility.Visible;

			int rHasDrumShip = fleet.Count(x => x.Slots.Any(y => y.Equipped && y.Item.Info.CategoryId == 30));
			int rTotalDrum = fleet.Sum(x => x.Slots.Count(y => y.Equipped && y.Item.Info.CategoryId == 30));

			if (rTotalDrum >= nTotalDrum && rHasDrumShip >= nHasDrumShip) result = true;
			return result;
		}
		private bool CompareExpeditionData(int Mission, Ship[] fleet)
		{
			this.vFlag = Visibility.Collapsed;
			this.vFlagType = Visibility.Collapsed;
			this.vNeed = Visibility.Collapsed;
			this.vTotal = Visibility.Collapsed;
			this.vDrum = Visibility.Collapsed;
			this.vFuel = Visibility.Collapsed;
			this.vArmo = Visibility.Collapsed;
			this.vResource = Visibility.Collapsed;

			if (fleet.Count() <= 0) return false;
			if (Mission < 1) return false;
			bool Chk = true;
			if (this.ShipTypeTable?.Count <= 0) Chk = false;
			if (Mission < 1) return false;
			this.ShipTypeTable = this.ChangeSpecialType(this.ShipTypeTable, Mission);


			this.nArmoLoss = LossResource(fleet, Mission, 1);
			this.nFuelLoss = LossResource(fleet, Mission);

			var NeedShipRaw = KanColleClient.Current.Translations.GetExpeditionData("FormedNeedShip", Mission).Split(';');
			var FLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", Mission);
			var TotalLevel = KanColleClient.Current.Translations.GetExpeditionData("TotalLv", Mission);
			var FlagShipType = KanColleClient.Current.Translations.GetExpeditionData("FlagShipType", Mission);
			StringBuilder strb = new StringBuilder();

			if (KanColleClient.Current.Translations.GetExpeditionData("DrumCount", Mission) != string.Empty) Chk = this.DrumCount(Mission, fleet);

			if (NeedShipRaw[0] == string.Empty) return false;
			else strb.Append("총" + Convert.ToInt32(NeedShipRaw[0]) + "(");

			if (fleet.Count() < Convert.ToInt32(NeedShipRaw[0])) Chk = false;

			if (FLv != string.Empty && FLv != "-")
			{
				int lv = Convert.ToInt32(FLv);
				if (fleet[0] != null && fleet[0].Level < lv) Chk = false;
				this.FlagLv = ("Lv" + lv);
				this.vFlag = Visibility.Visible;
			}
			if (TotalLevel != string.Empty)
			{
				int totallv = Convert.ToInt32(TotalLevel);
				if (fleet.Sum(x => x.Level) < totallv) Chk = false;
				this.TotalLv = ("Lv" + totallv);
				this.vTotal = Visibility.Visible;
			}
			if (FlagShipType != string.Empty)
			{
				int flagship = Convert.ToInt32(FlagShipType);
				if (fleet[0].Info.ShipType.Id != flagship) Chk = false;
				this.FlagType = (KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, false, null, flagship));
				this.vFlagType = Visibility.Visible;
			}

			Dictionary<int, int> ExpeditionTable = new Dictionary<int, int>();

			if (NeedShipRaw.Count() > 1)
			{
				var Ships = NeedShipRaw[1].Split(',');
				for (int i = 0; i < Ships.Count(); i++)
				{
					var shipInfo = Ships[i].Split('*');
					if (shipInfo.Count() > 1)
						ExpeditionTable.Add(Convert.ToInt32(shipInfo[0]), Convert.ToInt32(shipInfo[1]));
				}
				var list = ExpeditionTable.ToList();
				for (int i = 0; i < ExpeditionTable.Count; i++)
				{
					if (i == 0)
					{
						strb.Append(KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, false, null, list[i].Key) + "×" + list[i].Value);
					}
					else strb.Append("・" + KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, false, null, list[i].Key) + "×" + list[i].Value);
				}
				strb.Append(")");
				strb = strb.Replace("()", "");
				this.ShipTypeString = strb.ToString();
				if (this.ShipTypeString.Count() > 0) this.vNeed = Visibility.Visible;

				for (int i = 0; i < ExpeditionTable.Count; i++)
				{
					var test = ExpeditionTable.ToList();
					if (this.ShipTypeTable.ContainsKey(test[i].Key))
					{
						var Count = this.ShipTypeTable[test[i].Key];
						if (ExpeditionTable[test[i].Key] > this.ShipTypeTable[test[i].Key])
							Chk = false;
					}
					else Chk = false;
				}
			}
			return Chk;
		}
		#region Make & Replace List
		private Dictionary<int, int> MakeShipTypeTable(Ship[] source)
		{
			if (source.Count() <= 0) return new Dictionary<int, int>();

			var rawList = source.Select(x => x.Info.ShipType.Id);
			return rawList.Distinct()
				.ToDictionary(x => x, x => rawList.Count(y => y == x));
		}
		private Dictionary<int, int> ChangeSpecialType(Dictionary<int, int> list, int MissionNum)
		{
			Dictionary<int, int> templist = new Dictionary<int, int>(list ?? new Dictionary<int, int>());
			bool Checker = true;
			int SpecialCount = 1;
			while (Checker)
			{
				string SepcialElement = "Special";
				var temp = SepcialElement + SpecialCount.ToString();
				var specialData = KanColleClient.Current.Translations.GetExpeditionData(temp, MissionNum);

				if (specialData != string.Empty)
				{
					var splitData = specialData.Split(';');
					if (templist.ContainsKey(Convert.ToInt32(splitData[0])))
					{
						int tempCount = templist[Convert.ToInt32(splitData[0])];

						if (templist.ContainsKey(Convert.ToInt32(splitData[1]))) templist[Convert.ToInt32(splitData[1])] += tempCount;
						else templist.Add(Convert.ToInt32(splitData[1]), tempCount);

						templist.Remove(Convert.ToInt32(splitData[0]));
					}
				}
				else Checker = false;
				SpecialCount++;
			}

			return templist;
		}
		private List<int> MakeResultList()
		{
			List<int> temp = new List<int>();

			bool IsEnd = true;
			int i = 1;
			int ListCount = KanColleClient.Current.Translations.GetExpeditionListCount();


			while (IsEnd)
			{
				var TRName = KanColleClient.Current.Translations.GetExpeditionData("TR-Name", i);
				var FlagLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", i);

				i++;
				if (TRName != string.Empty && FlagLv != string.Empty) temp.Add(i - 1);
				if (temp.Count == ListCount) IsEnd = false;
			}

			return temp;
		}
		#endregion
	}
}
