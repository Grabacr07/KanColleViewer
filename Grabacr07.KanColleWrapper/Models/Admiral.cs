using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// プレイヤー (提督) を表します。
	/// </summary>
	public class Admiral : RawDataWrapper<kcsapi_basic>
	{
		public string MemberId
		{
			get { return this.RawData.api_member_id; }
		}

		public string Nickname
		{
			get { return this.RawData.api_nickname; }
		}


		#region Comment 変更通知プロパティ

		private string _Comment;

		public string Comment
		{
			get { return this._Comment; }
			set
			{
				if (this._Comment != value)
				{
					this._Comment = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Experience 変更通知プロパティ

		/// <summary>
		/// 提督経験値を取得します。
		/// </summary>
		private int _Experience;
		public int Experience
		{
			get { return this._Experience; }
			set
			{
				if (this._Experience != value)
				{
					this._Experience = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ExperienceForNexeLevel");
				}
			}
		}
		#endregion

		/// <summary>
		/// 次のレベルに上がるために必要な提督経験値を取得します。
		/// </summary>
		public int ExperienceForNexeLevel
		{
			get { return Models.Experience.GetAdmiralExpForNextLevel(this.Level, this.Experience); }
		}

		#region Level 変更通知プロパティ

		/// <summary>
		/// 艦隊司令部レベルを取得します。
		/// </summary>
		private int _Level;
		public int Level
		{
			get { return this._Level; }
			set
			{
				if (this._Level != value)
				{
					this._Level = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		/// <summary>
		/// 提督のランク名 (元帥, 大将, 中将, ...) を取得します。
		/// </summary>
		public string Rank
		{
			get { return Models.Rank.GetName(this.RawData.api_rank); }
		}

		/// <summary>
		/// 出撃時の勝利数を取得します。
		/// </summary>
		public int SortieWins
		{
			get { return this.RawData.api_st_win; }
		}

		/// <summary>
		/// 出撃時の敗北数を取得します。
		/// </summary>
		public int SortieLoses
		{
			get { return this.RawData.api_st_lose; }
		}

		/// <summary>
		/// 出撃時の勝率を取得します。
		/// </summary>
		public double SortieWinningRate
		{
			get
			{
				var battleCount = this.RawData.api_st_win + this.RawData.api_st_lose;
				if (battleCount == 0) return 0.0;
				return this.RawData.api_st_win / (double)battleCount;
			}
		}

		/// <summary>
		/// 司令部に所属できる艦娘の最大値を取得します。
		/// </summary>
		public int MaxShipCount
		{
			get { return this.RawData.api_max_chara; }
		}

		/// <summary>
		/// 司令部が保有できる装備アイテムの最大値を取得します。
		/// </summary>
		public int MaxSlotItemCount
		{
			get { return this.RawData.api_max_slotitem; }
		}


		internal Admiral(kcsapi_basic rawData)
			: base(rawData)
		{
			this.Comment = this.RawData.api_comment;
			this.Experience = this.RawData.api_experience;
			this.Level = this.RawData.api_level;
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Nickname = \"{1}\", Level = {2}, Rank = \"{3}\"", this.MemberId, this.Nickname, this.Level, this.Rank);
		}
	}
}
