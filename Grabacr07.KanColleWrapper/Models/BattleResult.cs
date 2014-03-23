using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class BattleResult : RawDataWrapper<kcsapi_battleresult>
	{
		/// <summary>
		/// この戦闘の作戦名を取得します。
		/// </summary>
		public string OperationName
		{
			get { return this.RawData.api_quest_name; }
		}

		/// <summary>
		/// 敵艦隊の名称を取得します。
		/// </summary>
		public string EnemyFleetName
		{
			get { return this.RawData.api_enemy_info.api_deck_name; }
		}

		/// <summary>
		/// 戦闘結果のランク (S/A/B/C/D/E) を取得します。
		/// </summary>
		public string Rank
		{
			get { return this.RawData.api_win_rank; }
		}

		/// <summary>
		/// この戦闘によって艦娘をドロップしたかどうかを示す値を取得します。
		/// </summary>
		public bool IsDrop
		{
			get { return this.RawData.api_get_ship != null; }
		}

		public BattleResult(kcsapi_battleresult rawData) : base(rawData) { }
	}
}
