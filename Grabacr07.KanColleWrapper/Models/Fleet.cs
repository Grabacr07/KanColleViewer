using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Windows;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 複数の艦娘によって編成される、単一の常設艦隊を表します。
	/// </summary>
	public class Fleet : DisposableNotifier, IIdentifiable
	{
		private readonly Homeport homeport;
		private Ship[] originalShips; // null も含んだやつ

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
					this.State.Condition.Name = value;
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

		public bool IsInSortie { get; private set; }

		public FleetState State { get; private set; }

		/// <summary>
		/// 艦隊の遠征に関するステータスを取得します。
		/// </summary>
		public Expedition Expedition { get; private set; }

		private Dictionary<int, int> ShipTypeTable { get; set; }
		private Dictionary<int, int> MakeShipTypeTable(Ship[] source)
		{
			Dictionary<int, int> temp = new Dictionary<int, int>();
			List<int> rawList = new List<int>();
			List<int> DistinctList = new List<int>();

			foreach (var ship in source)
			{
				rawList.Add(ship.Info.ShipType.Id);
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
		private bool CompareExpeditionData(int MissionNum)
		{
			var temp = KanColleClient.Current.Translations.GetExpeditionData("FormedNeedShip", MissionNum).Split(';');
            if (temp[0] == string.Empty) return false;

			int TotalCount = Convert.ToInt32(temp[0]);
			Dictionary<int, int> ExpeditionTable = new Dictionary<int, int>();
			bool Checker = false;

			if (temp.Count() > 1)
			{
				var Ships = temp[1].Split(',');
				for (int i = 0; i < Ships.Count(); i++)
				{
					var shipInfo = Ships[i].Split('*');
					ExpeditionTable.Add(Convert.ToInt32(shipInfo[0]), Convert.ToInt32(shipInfo[1]));
				}
				for (int i = 0; i < ExpeditionTable.Count; i++)
				{
					var test = ExpeditionTable.ToList();
					if (this.ShipTypeTable.ContainsKey(test[i].Key))
					{
						var Count = this.ShipTypeTable[test[i].Key];
						if (ExpeditionTable[test[i].Key] <= this.ShipTypeTable[test[i].Key])
							Checker = true;
						else Checker = false;
					}
				}
			}
			if (this.Ships.Count() >= TotalCount)
				Checker = true;
			else Checker = false;
			//MessageBox.Show(Checker.ToString());
			return Checker;
		}

		internal Fleet(Homeport parent, kcsapi_deck rawData)
		{
			this.homeport = parent;

			this.State = new FleetState(parent, this);
			this.Expedition = new Expedition(this);
			this.CompositeDisposable.Add(this.State);
			this.CompositeDisposable.Add(this.Expedition);

			this.Update(rawData);
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

		#region 艦の編成 (Change, Unset)

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

		#endregion

		#region 出撃 (Sortie, Homing)

		internal void Sortie()
		{
			if (!this.IsInSortie)
			{
				this.IsInSortie = true;
				this.State.Update();
			}
		}

		internal void Homing()
		{
			if (this.IsInSortie)
			{
				this.IsInSortie = false;
				this.State.Update();
			}
		}

		#endregion

		private void UpdateShips(Ship[] ships)
		{
			this.originalShips = ships;
			this.Ships = ships.Where(x => x != null).ToArray();
			this.ShipTypeTable = MakeShipTypeTable(this.Ships);
			bool test = CompareExpeditionData(19);

			this.State.Calculate();
			this.State.Update();
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Ships = {2}", this.Id, this.Name, this.Ships.Select(s => "\"" + s.Info.Name + "\"").ToString(","));
		}
	}
}
