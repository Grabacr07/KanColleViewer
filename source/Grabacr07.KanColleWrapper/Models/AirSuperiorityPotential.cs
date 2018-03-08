using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models
{
	[Flags]
	public enum AirSuperiorityCalculationOptions
	{
		Default = Maximum,

		Minimum = InternalProficiencyMinValue | Fighter,
		Maximum = InternalProficiencyMaxValue | Fighter | Attacker | SeaplaneBomber | JetFightingBomber,

		/// <summary>艦上戦闘機、水上戦闘機。</summary>
		Fighter = 0x0001,

		/// <summary>艦上攻撃機、艦上爆撃機。</summary>
		Attacker = 0x0002,

		/// <summary>水上爆撃機。</summary>
		SeaplaneBomber = 0x0004,

		/// <summary>噴式戦闘爆撃機。</summary>
		JetFightingBomber = 0x0008,

		/// <summary>内部熟練度最小値による計算。</summary>
		InternalProficiencyMinValue = 0x0100,

		/// <summary>内部熟練度最大値による計算。</summary>
		InternalProficiencyMaxValue = 0x0200,
	}

	public static class AirSuperiorityPotential
	{
		/// <summary>
		/// 艦娘の制空能力を計算します。
		/// </summary>
		public static int GetAirSuperiorityPotential(this Ship ship, AirSuperiorityCalculationOptions options = AirSuperiorityCalculationOptions.Default)
		{
			return ship.EquippedItems
				.Select(x => GetAirSuperiorityPotential(x.Item, x.Current, options))
				.Sum();
		}

		/// <summary>
		/// 装備と搭載数を指定して、スロット単位の制空能力を計算します。
		/// </summary>
		public static int GetAirSuperiorityPotential(this SlotItem slotItem, int onslot, AirSuperiorityCalculationOptions options = AirSuperiorityCalculationOptions.Default)
		{
			var calculator = slotItem.GetCalculator();
			return options.HasFlag(calculator.Options) && onslot >= 1
				? calculator.GetAirSuperiority(slotItem, onslot, options)
				: 0;
		}

		private static AirSuperiorityCalculator GetCalculator(this SlotItem slotItem)
		{
			switch (slotItem.Info.Type)
			{
				case SlotItemType.艦上戦闘機:
				case SlotItemType.水上戦闘機:
					return new FighterCalculator();

				case SlotItemType.艦上攻撃機:
				case SlotItemType.艦上爆撃機:
					return new AttackerCalculator();

				case SlotItemType.水上爆撃機:
					return new SeaplaneBomberCalculator();

				case SlotItemType.噴式戦闘爆撃機:
					return new JetFightingBomberCaluculator();

				default:
					return EmptyCalculator.Instance;
			}
		}

		private abstract class AirSuperiorityCalculator
		{
			public abstract AirSuperiorityCalculationOptions Options { get; }

			public int GetAirSuperiority(SlotItem slotItem, int onslot, AirSuperiorityCalculationOptions options)
			{
				// 装備の対空値とスロットの搭載数による制空値
				var airSuperiority = this.GetAirSuperiority(slotItem, onslot);

				// 装備の熟練度による制空値ボーナス
				airSuperiority += this.GetProficiencyBonus(slotItem, options);

				return (int)airSuperiority;
			}

			protected virtual double GetAirSuperiority(SlotItem slotItem, int onslot)
			{
				return slotItem.Info.AA * Math.Sqrt(onslot);
			}

			protected abstract double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options);
		}

		#region AirSuperiorityCalculator 派生型

		private class FighterCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Fighter;

			protected override double GetAirSuperiority(SlotItem slotItem, int onslot)
			{
				// 装備改修による対空値加算 (★ x 0.2)
				return (slotItem.Info.AA + slotItem.Level * 0.2) * Math.Sqrt(onslot);
			}

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.FighterBonus;
			}
		}

		private class AttackerCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Attacker;

			protected override double GetAirSuperiority(SlotItem slotItem, int onslot)
			{
				// 爆戦の装備改修による対空値加算 (★ x 0.25)
				return (slotItem.Info.AA + slotItem.Level * 0.25) * Math.Sqrt(onslot);
			}

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0);
			}
		}

		private class SeaplaneBomberCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.SeaplaneBomber;

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.SeaplaneBomberBonus;
			}
		}

		private class JetFightingBomberCaluculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.JetFightingBomber;

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0);
			}
		}


		private class EmptyCalculator : AirSuperiorityCalculator
		{
			public static EmptyCalculator Instance { get; } = new EmptyCalculator();

			public override AirSuperiorityCalculationOptions Options => ~AirSuperiorityCalculationOptions.Default;
			protected override double GetAirSuperiority(SlotItem slotItem, int onslot) => .0;
			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options) => .0;

			private EmptyCalculator() { }
		}

		#endregion

		private class Proficiency
		{
			private readonly int internalMinValue;
			private readonly int internalMaxValue;

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

		private static readonly Dictionary<int, Proficiency> proficiencies = new Dictionary<int, Proficiency>()
		{
			{ 0, new Proficiency(0, 9, 0, 0) },
			{ 1, new Proficiency(10, 24, 0, 0) },
			{ 2, new Proficiency(25, 39, 2, 1) },
			{ 3, new Proficiency(40, 54, 5, 1) },
			{ 4, new Proficiency(55, 69, 9, 1) },
			{ 5, new Proficiency(70, 84, 14, 3) },
			{ 6, new Proficiency(85, 99, 14, 3) },
			{ 7, new Proficiency(100, 120, 22, 6) },
		};

		private static Proficiency GetProficiency(this SlotItem slotItem) => proficiencies[Math.Max(Math.Min(slotItem.Proficiency, 7), 0)];
	}
}
