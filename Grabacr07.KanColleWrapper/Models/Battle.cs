using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class Battle : RawDataWrapper<kcsapi_battle>
	{
		/// <summary>
		/// 敵艦隊に属する艦を取得します。
		/// </summary>
		public EnemyShip[] Enemies { get; private set; }

		public Battle(kcsapi_battle rawData)
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
	}
}
