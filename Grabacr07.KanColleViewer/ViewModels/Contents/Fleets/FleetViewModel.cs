using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Windows;
using Grabacr07.KanColleWrapper;
using System.Text;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 単一の艦隊情報を提供します。
	/// </summary>
	public class FleetViewModel : ItemViewModel
	{
		public Fleet Source { get; private set; }
		public int Id
		{
			get
			{
				return this.Source.Id;
			}
		}
		private Dictionary<int, int> ShipTypeTable { get; set; }
		public string Name
		{
			get { return string.IsNullOrEmpty(this.Source.Name.Trim()) ? "(第 " + this.Source.Id + " 艦隊)" : this.Source.Name; }
		}

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

		#region 선택된 원정번호
		private string _ExpeditionId;
		public string ExpeditionId
		{
			get { return this._ExpeditionId; }
			set
			{
				if (this._ExpeditionId == value) return;
				this._ExpeditionId = value;
				this.IsPassed = this.CompareExpeditionData(value, this.Ships);
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
		#endregion

		/// <summary>
		/// 艦隊に所属している艦娘のコレクションを取得します。
		/// </summary>
		public ShipViewModel[] Ships
		{
			get
			{
				ShipViewModel[] temps = this.Source.Ships.Select(x => new ShipViewModel(x)).ToArray();
				this.ShipTypeTable = MakeShipTypeTable(temps);

				this.IsPassed = CompareExpeditionData(this.ExpeditionId, temps);

				return temps;
			}
		}
		public List<int> ResultList { get; set; }

		public FleetStateViewModel State { get; private set; }

		public ExpeditionViewModel Expedition { get; private set; }


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

		public FleetViewModel(Fleet fleet)
		{
			this.Source = fleet;
			this.IsFirstFleet = Visibility.Collapsed;

			if (this.Source.Id > 1)
			{
				IsFirstFleet = Visibility.Visible;

				this.ResultList = this.MakeResultList();
				this.ExpeditionId = this.ResultList.First().ToString();
			}
			else IsFirstFleet = Visibility.Collapsed;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ "Situation", (sender, args) => this.RaisePropertyChanged("QuickStateView") },
			});

			this.State = new FleetStateViewModel(fleet.State);
			this.CompositeDisposable.Add(this.State);

			this.Expedition = new ExpeditionViewModel(fleet.Expedition);
			this.CompositeDisposable.Add(this.Expedition);
		}
		private Dictionary<int, int> ChangeSpecialType(Dictionary<int, int> list, int MissionNum)
		{
			Dictionary<int, int> templist = new Dictionary<int, int>(list);
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
		private bool CompareExpeditionData(string Mission, ShipViewModel[] fleet)
		{
			this.vFlag = Visibility.Collapsed;
			this.vFlagType = Visibility.Collapsed;
			this.vNeed = Visibility.Collapsed;
			this.vTotal = Visibility.Collapsed;

			if (fleet.Count() <= 0) return false;
			if (Mission == null) return false;
			bool Chk = true;
			int MissionNum = 0;
			try
			{
				MissionNum = Convert.ToInt32(Mission);
			}
			catch
			{
				return false;
			}
			if (this.ShipTypeTable.Count <= 0) Chk = false;
			if (MissionNum < 1) return false;
			this.ShipTypeTable = this.ChangeSpecialType(this.ShipTypeTable, MissionNum);

			var NeedShipRaw = KanColleClient.Current.Translations.GetExpeditionData("FormedNeedShip", MissionNum).Split(';');
			var FLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", MissionNum);
			var TotalLevel = KanColleClient.Current.Translations.GetExpeditionData("TotalLv", MissionNum);
			var FlagShipType = KanColleClient.Current.Translations.GetExpeditionData("FlagShipType", MissionNum);


			if (NeedShipRaw[0] == string.Empty) Chk = false;
			if (fleet.Count() < Convert.ToInt32(NeedShipRaw[0])) Chk = false;
			if (FLv != string.Empty && FLv != "-")
			{
				int lv = Convert.ToInt32(FLv);
				if (fleet[0] != null && fleet[0].Ship.Level < lv) Chk = false;
				this.FlagLv = ("Lv" + lv);
				this.vFlag = Visibility.Visible;
			}
			if (TotalLevel != string.Empty)
			{
				int totallv = Convert.ToInt32(TotalLevel);
				if (fleet.Sum(x => x.Ship.Level) < totallv) Chk = false;
				this.TotalLv = ("Lv" + totallv);
				this.vTotal = Visibility.Visible;
			}
			if (FlagShipType != string.Empty)
			{
				int flagship = Convert.ToInt32(FlagShipType);
				if (fleet[0].Ship.Info.ShipType.Id != flagship) Chk = false;
				this.FlagType = (KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, null, flagship));
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
				StringBuilder strb = new StringBuilder();
				for (int i = 0; i < ExpeditionTable.Count; i++)
				{
					if (i == 0)
					{
						strb.Append(KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, null, list[i].Key) + "×" + list[i].Value);
					}
					else strb.Append("・" + KanColleClient.Current.Translations.GetTranslation("", TranslationType.ShipTypes, null, list[i].Key) + "×" + list[i].Value);
				}

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
		private Dictionary<int, int> MakeShipTypeTable(ShipViewModel[] source)
		{
			if (source.Count() <= 0) return new Dictionary<int, int>();
			Dictionary<int, int> temp = new Dictionary<int, int>();
			List<int> rawList = new List<int>();
			List<int> DistinctList = new List<int>();

			foreach (var ship in source)
			{
				int ID = ship.Ship.Info.ShipType.Id;
				rawList.Add(ID);
			}
			for (int i = 0; i < rawList.Count; i++)
			{
				if (DistinctList.Contains(rawList[i]))
					continue;
				DistinctList.Add(rawList[i]);
			}
			for (int i = 0; i < DistinctList.Count; i++)
			{
				temp.Add(DistinctList[i], rawList.Where(x => x == DistinctList[i]).Count());
			}
			return temp;
		}
	}
}
