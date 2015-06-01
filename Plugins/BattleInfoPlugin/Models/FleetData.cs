using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Grabacr07.KanColleWrapper.Models;

namespace BattleInfoPlugin.Models
{
	public class FleetData : NotificationObject
	{

		#region Name変更通知プロパティ
		private string _Name;

		public string Name
		{
			get
			{ return this._Name; }
			set
			{
				if (this._Name == value)
					return;
				this._Name = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region AttackGauge変更通知プロパティ
		private string _AttackGauge;

		public string AttackGauge
		{
			get
			{ return this._AttackGauge; }
			set
			{
				if (this._AttackGauge == value)
					return;
				this._AttackGauge = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region IsCritical変更通知プロパティ
		private bool _IsCritical;

		public bool IsCritical
		{
			get
			{ return this._IsCritical; }
			set
			{
				if (this._IsCritical == value)
					return;
				this._IsCritical = value;
			}
		}
		#endregion

		#region TotalDamaged変更通知プロパティ
		private int _TotalDamaged;

		public int TotalDamaged
		{
			get
			{ return this._TotalDamaged; }
			set
			{
				if (value == 0) this.SinkCount = 0;
				if (this._TotalDamaged == value)
					return;
				this._TotalDamaged = value;
			}
		}
		#endregion

		#region SinkCount変更通知プロパティ
		private int _SinkCount;

		public int SinkCount
		{
			get
			{ return this._SinkCount; }
			set
			{
				if (this._SinkCount == value)
					return;
				this._SinkCount = value;
			}
		}
		#endregion

		#region Ships変更通知プロパティ
		private IEnumerable<ShipData> _Ships;

		public IEnumerable<ShipData> Ships
		{
			get
			{ return this._Ships; }
			set
			{
				if (this._Ships == value)
					return;
				this._Ships = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Formation変更通知プロパティ
		private Formation _Formation;

		public Formation Formation
		{
			get
			{ return this._Formation; }
			set
			{
				if (this._Formation == value)
					return;
				this._Formation = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region AirSuperiorityPotential変更通知プロパティ
		private int _AirSuperiorityPotential;

		public int AirSuperiorityPotential
		{
			get
			{ return this._AirSuperiorityPotential; }
			set
			{
				if (this._AirSuperiorityPotential == value)
					return;
				this._AirSuperiorityPotential = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		public int AirParityRequirements { get { return this.AirSuperiorityPotential * 2 / 3; } }
		public int AirSuperiorityRequirements { get { return this.AirSuperiorityPotential * 3 / 2; } }
		public int AirSupremacyRequirements { get { return this.AirSuperiorityPotential * 3; } }

		public FleetData()
			: this(new ShipData[0], Formation.없음, "")
		{
		}

		public FleetData(IEnumerable<ShipData> ships, Formation formation, string name)
		{
			this._Ships = ships;
			this._Formation = formation;
			this._Name = name;
			this._AirSuperiorityPotential = this._Ships
				.SelectMany(s => s.Slots)
				.Where(s => s.Source.IsAirSuperiorityFighter)
				.Sum(s => (int)(s.AA * Math.Sqrt(s.Current)))
				;
		}
	}

	public static class FleetDataExtensions
	{
		/// <summary>
		/// Actionを使用して値を設定
		/// Zipするので要素数が少ない方に合わせられる
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source"></param>
		/// <param name="values"></param>
		/// <param name="setter"></param>
		public static void SetValues<TSource, TValue>(
			this IEnumerable<TSource> source,
			IEnumerable<TValue> values,
			Action<TSource, TValue> setter)
		{
			source.Zip(values, (s, v) => new { s, v })
				.ToList()
				.ForEach(x => setter(x.s, x.v));
		}
		/// <summary>
		/// ダメージ適用
		/// </summary>
		/// <param name="fleet">艦隊</param>
		/// <param name="damages">適用ダメージリスト</param>
		public static void CalcDamages(this FleetData fleet, params FleetDamages[] damages)
		{
			foreach (var damage in damages)
			{
				fleet.Ships.SetValues(damage.ToArray(), (s, d) => s.NowHP -= d);
			}

			if (fleet.Ships == null) return;
			foreach (var item in fleet.Ships)
			{
				if (!item.Situation.HasFlag(ShipSituation.Evacuation) && !item.Situation.HasFlag(ShipSituation.Tow))
				{
					int tempHP = item.NowHP;
					if (item.NowHP < 0) tempHP = 0;
					fleet.TotalDamaged += (item.BeforeNowHP - tempHP);
				}
				if (item.NowHP <= 0) fleet.SinkCount++;
			}
			foreach (var item in fleet.Ships)//데미지 총합 합계
			{
				if (item.MaxHP > 0)
				{
					if ((item.NowHP / (double)item.MaxHP) <= 0.25)
					{
						if (!item.Situation.HasFlag(ShipSituation.DamageControlled) &&
							!item.Situation.HasFlag(ShipSituation.Evacuation) &&
							!item.Situation.HasFlag(ShipSituation.Tow))
						{
							fleet.IsCritical = true;
							break;
						}
						else fleet.IsCritical = false;
					}
					else fleet.IsCritical = false;
				}
			}
		}
		public static bool CriticalCheck(this FleetData fleet)
		{
			if (fleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.DamageControlled))
					.Where(x => !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow))
					.Any(x => x.Situation.HasFlag(ShipSituation.HeavilyDamaged)))
				return true;
			else return false;
		}
	}
}
