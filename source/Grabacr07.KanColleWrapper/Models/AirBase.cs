using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class AirBase : RawDataWrapper<kcsapi_airbase_corps>, IIdentifiable
	{
		private Homeport homeport { get; }

		public int Id => this.RawData.api_rid;
		public string Name => this.RawData.api_name;
		public int Distance => this.RawData.api_distance;
		public AirBaseAction ActionKind => (AirBaseAction)this.RawData.api_action_kind;

		public AirBasePlane[] Planes { get; private set; }
		public double AttackPower { get; private set; }
		public double AirSuperiorityPotential { get; private set; }

		public string AttackPowerText => this.AttackPower.ToString("0.#");
		public string AirSuperiorityPotentialText => this.AirSuperiorityPotential.ToString("0.#");

		public AirBase(kcsapi_airbase_corps rawData, Homeport homeport) : base(rawData)
		{
			this.homeport = homeport;
			this.Update(RawData);
		}

		public void Update(kcsapi_airbase_corps rawData)
		{
			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
			this.RaisePropertyChanged(nameof(this.Planes));
			CalculateLandBased();
		}
		public void Update(kcsapi_airbase_corps_set_plane rawData)
		{
			this.RawData.api_distance = rawData.api_distance;

			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
			this.RaisePropertyChanged(nameof(this.Planes));
			CalculateLandBased();
		}
		public void Update(kcsapi_airbase_corps_supply rawData)
		{
			this.RawData.api_distance = rawData.api_distance;

			var array = new AirBasePlane[4];
			foreach (var item in this.RawData.api_plane_info)
				array[item.api_squadron_id - 1] = new AirBasePlane(item, homeport.Itemyard.SlotItems[item.api_slotid]);

			this.Planes = array;
			this.RaisePropertyChanged(nameof(this.Planes));
			CalculateLandBased();
		}
		public void UpdateActionKind(AirBaseAction action)
		{
			this.RawData.api_action_kind = (int)action;
			this.RaisePropertyChanged(nameof(this.ActionKind));

			CalculateLandBased();
		}

		private void CalculateLandBased()
		{
			Dictionary<int, int[]> distanceBonus = new Dictionary<int, int[]>()
			{
				{ 138, new int[] { 3, 3, 3, 3, 3, 3, 3, 3 } }, // 이식대정
				{ 178, new int[] { 3, 3, 2, 2, 2, 2, 1, 1 } }, // 카탈리나
				{ 151, new int[] { 2, 2, 2, 2, 1, 1, 0, 0 } }, // 시제케이운
				{  54, new int[] { 2, 2, 2, 2, 1, 1, 0, 0 } }, // 사이운
				{  25, new int[] { 2, 2, 2, 1, 1, 0, 0, 0 } }, // 영식수상정찰기
				{  59, new int[] { 2, 2, 2, 1, 1, 0, 0, 0 } }, // 영식수상관측기
				{  61, new int[] { 2, 1, 1, 0, 0, 0, 0, 0 } }, // 2식함상정찰기
			};
			Dictionary<int, Proficiency> proficiencies = new Dictionary<int, Proficiency>()
			{
				{ 0, new Proficiency(  0,   9,  0,  0) },
				{ 1, new Proficiency( 10,  24,  0,  1) },
				{ 2, new Proficiency( 25,  39,  2,  2) },
				{ 3, new Proficiency( 40,  54,  5,  3) },
				{ 4, new Proficiency( 55,  69,  9,  4) },
				{ 5, new Proficiency( 70,  84, 14,  5) },
				{ 6, new Proficiency( 85,  99, 14,  7) },
				{ 7, new Proficiency(100, 120, 22,  9) },
			};
			var def = AirSuperiorityCalculationOptions.Default;

			var items = this.Planes
				.Where(x => (x?.MaximumCount ?? 0) > 0)
				.Select(x => x.Source);

			AttackPower = 0;
			AirSuperiorityPotential = 0;
			if (items.Count() == 0) return;

			#region Attack Power calculating
			var attackers = new SlotItemType[]
			{
				SlotItemType.艦上攻撃機,
				SlotItemType.艦上爆撃機,
				SlotItemType.噴式攻撃機,
				SlotItemType.噴式戦闘爆撃機,
				SlotItemType.陸上攻撃機,
				SlotItemType.水上爆撃機
			};
			var power_sum = items
				.Where(x => attackers.Contains(x.Info.Type))
				.Sum(item =>
				{
					var proficiency = proficiencies[item.Proficiency];
					double damage = 0;

					if (item.Info.Type == SlotItemType.陸上攻撃機)
					{
						damage = (item.Info.Torpedo + item.Info.Bomb) / 2; // P
						damage *= Math.Sqrt(1.8 * 18); // root 1.8N
						damage += 25;
						damage = Math.Floor(damage * 0.8);
						// Critical modifier skip
						// Contact multiplier skip
					}
					else
					{
						damage = (item.Info.Torpedo + item.Info.Bomb) / 2; // P
						damage *= Math.Sqrt(1.8 * 18); // root 1.8N
						damage += 25;
						damage = Math.Floor(damage * 0.8);
						// Critical modifier skip
						// Contact multiplier skip
						damage *= 1.8;
					}
					return damage;
				});
			#endregion

			#region Bonus rate calculate when Air Defence Mode
			var bonusRate = 1.0;
			if (this.ActionKind == AirBaseAction.방공)
			{
				if (items.Any(x => x.Info.Type == SlotItemType.艦上偵察機))
				{
					var viewrange = items
						.Where(x => x.Info.Type == SlotItemType.艦上偵察機)
						.Max(x => x.Info.ViewRange);

					if (viewrange <= 7)
						bonusRate = 1.2;
					else if (viewrange == 8)
						bonusRate = 1.25; // Maybe?
					else
						bonusRate = 1.3;
				}
				else if (items.Any(x => x.Info.Type == SlotItemType.水上偵察機))
				{
					var viewrange = items
						.Where(x => x.Info.Type == SlotItemType.水上偵察機)
						.Max(x => x.Info.ViewRange);

					if (viewrange <= 7)
						bonusRate = 1.1;
					else if (viewrange == 8)
						bonusRate = 1.13;
					else
						bonusRate = 1.16;
				}
			}
			#endregion
			#region AA calculating
			var air_sum = items.Sum(item =>
			{
				var proficiency = proficiencies[item.Proficiency];
				double aa = item.Info.AA;
				double bonus = 0;

				switch (item.Info.Type)
				{
					// 전투기
					case SlotItemType.艦上戦闘機:
					case SlotItemType.水上戦闘機:
					case SlotItemType.噴式戦闘機:
					case SlotItemType.局地戦闘機:
						aa += item.Level * 0.2;
						bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
							+ proficiency.FighterBonus;
						break;

					// 공격기 (뇌격기, 폭격기)
					case SlotItemType.艦上攻撃機:
					case SlotItemType.艦上爆撃機:
					case SlotItemType.噴式攻撃機:
					case SlotItemType.噴式戦闘爆撃機:
					case SlotItemType.陸上攻撃機:
						bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
							+ 0;
						break;

					// 수상폭격기
					case SlotItemType.水上爆撃機:
						bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
							+ proficiency.SeaplaneBomberBonus;
						break;

					// 정찰기, 수상정찰기, (분식정찰기?)
					// 본래는 제공치에 포함되지 않으나 기항대에는 포함되는 듯
					// 다만 기지항공대 공격을 방공할 때에만 출격하는 듯함. 일단은 주석처리
					default:
						/*
						bonus = Math.Sqrt(proficiency.GetInternalValue(def) / 10.0)
							+ 0;
						*/
						break;
				}
				bonus = Math.Min(22, bonus) + Math.Sqrt(12);

				switch (this.ActionKind)
				{
					case AirBaseAction.방공:
						if (item.Info.Type == SlotItemType.局地戦闘機)
							aa += item.Info.Hit * 2 + item.Info.Evade;
						break;
					default: // 출격 등
						if (item.Info.Type == SlotItemType.局地戦闘機)
							aa += (int)(item.Info.Evade * 1.5);
						break;
				}
				return Math.Floor(Math.Sqrt(18) * aa) * bonusRate + Math.Floor(bonus);
			});
			#endregion

			this.AttackPower = power_sum;
			this.AirSuperiorityPotential = air_sum;
		}

		private class Proficiency
		{
			private int internalMinValue { get; }
			private int internalMaxValue { get; }

			public int FighterBonus { get; }
			public int SeaplaneBomberBonus { get; }

			public Proficiency(int internalMin, int internalMax, int fighterBonus, int seaplaneBomberBonus)
			{
				this.internalMinValue = internalMin;
				this.internalMaxValue = internalMax;
				this.FighterBonus = fighterBonus;
				this.SeaplaneBomberBonus = seaplaneBomberBonus;
			}

			/// <summary>
			/// 内部熟練度値を取得します。
			/// </summary>
			public int GetInternalValue(AirSuperiorityCalculationOptions options)
			{
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMinValue)) return this.internalMinValue;
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMaxValue)) return this.internalMaxValue;
				return (this.internalMaxValue + this.internalMinValue) / 2; // <- めっちゃ適当
			}
		}
	}

	public class AirBasePlane : RawDataWrapper<kcsapi_plane_info>, IIdentifiable
	{
		public SlotItem Source { get; }

		public int Id => this.RawData.api_squadron_id;
		public string Name => this.Source?.NameWithLevel ?? "";
		public int State => this.RawData.api_state;

		public int MaximumCount => this.RawData.api_max_count;
		public int CurrentCount => this.RawData.api_count;
		public int LostCount => this.MaximumCount - this.CurrentCount;

		public int Condition => this.RawData.api_cond;
		public ConditionType ConditionIcon => (ConditionType)this.RawData.api_cond;

		public AirBasePlane(kcsapi_plane_info rawData, SlotItem slotitem) : base(rawData)
		{
			this.Source = slotitem;
		}
	}

	public enum AirBaseAction
	{
		대기 = 0,
		출격 = 1,
		방공 = 2,
		퇴피 = 3,
		휴식 = 4
	}
}
