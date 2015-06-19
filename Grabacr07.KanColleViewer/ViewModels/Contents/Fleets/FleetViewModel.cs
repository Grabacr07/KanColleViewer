using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Windows;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 単一の艦隊情報を提供します。
	/// </summary>
	public class FleetViewModel : ItemViewModel
	{
		public Fleet Source { get; private set; }
		public Visibility IsFirstFleet { get; set; }
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
		public List<int> ResultList
		{
			get;
			set;
		}

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


			if (this.Source.Id != 1)
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
		private bool CompareExpeditionData(string Mission, ShipViewModel[] fleet)
		{
			if (fleet.Count() <= 0) return false;
			int MissionNum = 0;
			try
			{
				MissionNum = Convert.ToInt32(Mission);
			}
			catch
			{
				return false;
			}
			if (this.ShipTypeTable.Count <= 0) return false;
			var temp = KanColleClient.Current.Translations.GetExpeditionData("FormedNeedShip", MissionNum).Split(';');
			if (temp[0] == string.Empty) return false;

			int TotalCount = Convert.ToInt32(temp[0]);
			Dictionary<int, int> ExpeditionTable = new Dictionary<int, int>();

			if (temp.Count() > 1)
			{
				var Ships = temp[1].Split(',');
				for (int i = 0; i < Ships.Count(); i++)
				{
					var shipInfo = Ships[i].Split('*');
					if (shipInfo.Count() > 1)
						ExpeditionTable.Add(Convert.ToInt32(shipInfo[0]), Convert.ToInt32(shipInfo[1]));
					else return false;
				}
				for (int i = 0; i < ExpeditionTable.Count; i++)
				{
					var test = ExpeditionTable.ToList();
					if (this.ShipTypeTable.ContainsKey(test[i].Key))
					{
						var Count = this.ShipTypeTable[test[i].Key];
						if (ExpeditionTable[test[i].Key] > this.ShipTypeTable[test[i].Key])
							return false;
					}
					else return false;
				}
			}
			if (fleet.Count() < TotalCount)
				return false;

			var FlagLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", MissionNum);
			if (FlagLv != string.Empty && FlagLv != "-")
			{
				int lv = Convert.ToInt32(FlagLv);
				if (fleet[0] != null && fleet[0].Ship.Level < lv) return false;
			}
			var TotalLevel = KanColleClient.Current.Translations.GetExpeditionData("TotalLv", MissionNum);
			if (TotalLevel != string.Empty)
			{
				int totallv = Convert.ToInt32(TotalLevel);
				if (fleet.Sum(x => x.Ship.Level) < totallv) return false;
			}
			var FlagShipType = KanColleClient.Current.Translations.GetExpeditionData("FlagShipType", MissionNum);
			if (FlagShipType != string.Empty)
			{
				int flagship = Convert.ToInt32(FlagShipType);
				if (fleet[0].Ship.Info.ShipType.Id != flagship) return false;
			}
			return true;
		}
		private Dictionary<int, int> MakeShipTypeTable(ShipViewModel[] source)
		{
			if (source.Count() <= 0) return new Dictionary<int, int>();
			Dictionary<int, int> temp = new Dictionary<int, int>();
			List<int> rawList = new List<int>();
			List<int> DistinctList = new List<int>();

			foreach (var ship in source)
			{
				rawList.Add(ship.Ship.Info.ShipType.Id);
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
