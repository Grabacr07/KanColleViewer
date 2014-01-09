using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 近代化改修による数値の上昇が可能なステータスを表します。
	/// </summary>
	public struct ModernizableStatus
	{
		/// <summary>
		/// 艦娘ごとに定義されたステータスの最大値を取得します。
		/// </summary>
		public int Max { get; internal set; }

		/// <summary>
		/// 艦娘ごとに定義されたステータスの初期値を取得します。
		/// </summary>
		public int Default { get; internal set; }

		/// <summary>
		/// 近代化改修による現在の上昇値を取得します。
		/// </summary>
		public int Upgraded { get; internal set; }

		/// <summary>
		/// 近代化改修によって上昇した分を含む現在のステータス値を取得します。
		/// </summary>
		public int Current
		{
			get { return this.Default + this.Upgraded; }
		}

		/// <summary>
		/// このステータスが上限に達するのに必要な値を取得します。
		/// </summary>
		public int Shortfall
		{
			get { return this.Max - this.Current; }
		}

		/// <summary>
		/// このステータスが上限に達しているかどうかを示す値を取得します。
		/// </summary>
		public bool IsMax
		{
			get { return this.Max <= this.Current; }
		}


		internal ModernizableStatus(int[] status, int upgraded)
			: this()
		{
			if (status.Length == 2)
			{
				this.Default = status[0];
				this.Max = status[1];
			}

			this.Upgraded = upgraded;
		}

		public override string ToString()
		{
			return string.Format("Status = {0}->{1}, Current = {2}{3}", this.Default, this.Max, this.Current, this.IsMax ? "(max)" : "");
		}


		#region static members

		private static readonly ModernizableStatus dummy = new ModernizableStatus(new[] { -1, -1 }, 0);

		public static ModernizableStatus Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
