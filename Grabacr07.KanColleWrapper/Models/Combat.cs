using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class Combat : RawDataWrapper<kcsapi_battle>
	{
		/// <summary>
		/// この戦闘で、先制雷撃が行われたかどうかを示す値を取得します。
		/// </summary>
		public bool HasOpeningAttack
		{
			get { return this.RawData.api_opening_flag == 1; }
		}

		/// <summary>
		/// この戦闘で、支援艦隊による支援攻撃が行われたかどうかを示す値を取得します。
		/// </summary>
		public bool HasSupportAttack
		{
			get { return this.RawData.api_support_flag == 1; }
		}

		/// <summary>
		/// 交戦形態を取得します。
		/// </summary>
		public CombatEngagementForm EngagementForm
		{
			get { return (CombatEngagementForm)(this.RawData.api_formation.Get(2) ?? 0); }
		}

		/// <summary>
		/// 敵艦隊に属する艦を取得します。
		/// </summary>
		public EnemyShip[] Enemies { get; private set; }


		public Combat(kcsapi_battle rawData)
			: base(rawData)
		{
			this.Enemies = rawData.api_ship_ke
				.Skip(1) // なんで最初に -1 入ってるんだろうね？
				.Where(id => id != -1)
				.Select((id, i) => new EnemyShip
				{
					Info = KanColleClient.Current.Master.Ships[id],
					SlotItems = rawData.api_eSlot[i]
						.Where(x => x != -1)
						.Select(x => KanColleClient.Current.Master.SlotItems[x])
						.ToArray(),
					MaxHP = rawData.api_maxhps[i + 7], // 謎の先頭 "-1" + 味方艦 6 隻分を除いて、7 番目から
					Firepower = rawData.api_eParam[i][0],
					Torpedo = rawData.api_eParam[i][1],
					AA = rawData.api_eParam[i][2],
					Armer = rawData.api_eParam[i][3],
				})
				.ToArray();
		}
	}


	/// <summary>
	/// 戦闘する敵艦を表します。
	/// </summary>
	public class EnemyShip
	{
		/// <summary>
		/// 敵艦のマスター情報を取得します。
		/// </summary>
		public ShipInfo Info { get; internal set; }

		/// <summary>
		/// 敵艦の各装備のマスター情報を取得します。
		/// </summary>
		public SlotItemInfo[] SlotItems { get; internal set; }

		/// <summary>
		/// 敵艦の最大耐久値を取得します。
		/// </summary>
		public int MaxHP { get; internal set; }

		/// <summary>
		/// 火力ステータス値を取得します。
		/// </summary>
		public int Firepower { get; internal set; }

		/// <summary>
		/// 雷装ステータス値を取得します。
		/// </summary>
		public int Torpedo { get; internal set; }

		/// <summary>
		/// 対空ステータス値を取得します。
		/// </summary>
		public int AA { get; internal set; }

		/// <summary>
		/// 装甲ステータス値を取得します。
		/// </summary>
		public int Armer { get; internal set; }


		public override string ToString()
		{
			return string.Format("{0}, HP: {1}, Slot: {2}", this.Info.Name, this.MaxHP, this.SlotItems.Select(x => x.Name).ToString("/"));
		}
	}
}
