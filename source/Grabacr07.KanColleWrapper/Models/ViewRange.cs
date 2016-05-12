using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 索敵値計算を提供します。
	/// </summary>
	public interface ICalcViewRange
	{
		string Id { get; }

		string Name { get; }

		string Description { get; }

		bool HasCombinedSettings { get; }

		double Calc(Fleet[] fleets);
	}


	public abstract class ViewRangeCalcLogic : ICalcViewRange
	{
		private static readonly Dictionary<string, ICalcViewRange> logics = new Dictionary<string, ICalcViewRange>();

		public static IEnumerable<ICalcViewRange> Logics => logics.Values;

		public static ICalcViewRange Get(string key)
		{
			ICalcViewRange logic;
			return logics.TryGetValue(key, out logic) ? logic : new ViewRangeType1();
		}

		static ViewRangeCalcLogic()
		{
			// ひどぅい設計を見た
			// ReSharper disable ObjectCreationAsStatement
			new ViewRangeType1();
			new ViewRangeType2();
			new ViewRangeType3();
			new ViewRangeType4();
			// ReSharper restore ObjectCreationAsStatement
		}

		public abstract string Id { get; }
		public abstract string Name { get; }
		public abstract string Description { get; }
		public virtual bool HasCombinedSettings { get; } = false;
		public abstract double Calc(Fleet[] fleets);

		protected ViewRangeCalcLogic()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			var key = this.Id;
			if (key != null && !logics.ContainsKey(key)) logics.Add(key, this);
		}
	}


	public class ViewRangeType1 : ViewRangeCalcLogic
	{
		public override sealed string Id => "KanColleViewer.Type1";

		public override string Name => "単純計算";

		public override string Description => "艦娘と装備の索敵値の単純な合計値";

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;

			return fleets.SelectMany(x => x.Ships).Sum(x => x.ViewRange);
		}
	}


	public class ViewRangeType2 : ViewRangeCalcLogic
	{
		public override sealed string Id => "KanColleViewer.Type2";

		public override string Name => "2-5 式 (旧)";

		public override string Description => "(偵察機 × 2) + (電探) + √(装備込みの艦隊索敵値合計 - 偵察機 - 電探)";

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;
			var ships = fleets.SelectMany(x => x.Ships).ToArray();

			// http://wikiwiki.jp/kancolle/?%C6%EE%C0%BE%BD%F4%C5%E7%B3%A4%B0%E8#area5
			// [索敵装備と装備例] によって示されている計算式
			// stype=7 が偵察機 (2 倍する索敵値)、stype=8 が電探

			var spotter = ships.SelectMany(
				x => x.EquippedItems
					.Where(s => s.Item.Info.RawData.api_type.Get(1) == 7)
					.Where(s => s.Current > 0)
					.Select(s => s.Item.Info.RawData.api_saku)
				).Sum();

			var radar = ships.SelectMany(
				x => x.EquippedItems
					.Where(s => s.Item.Info.RawData.api_type.Get(1) == 8)
					.Select(s => s.Item.Info.RawData.api_saku)
				).Sum();

			return (spotter * 2) + radar + (int)Math.Sqrt(ships.Sum(x => x.ViewRange) - spotter - radar);
		}
	}


	public class ViewRangeType3 : ViewRangeCalcLogic
	{
		public override sealed string Id => "KanColleViewer.Type3";

		public override string Name => "2-5 式 (秋)";

		public override string Description => @"(艦上爆撃機 × 1.04) + (艦上攻撃機 × 1.37) + (艦上偵察機 × 1.66)
+ (水上偵察機 × 2.00) + (水上爆撃機 × 1.78) + (探照灯 × 0.91)
+ (小型電探 × 1.00) + (大型電探 × .99) + (√各艦毎の素索敵 × 1.69)
+ (司令部レベルを 5 の倍数に切り上げ × -0.61)";

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;
			var ships = fleets.SelectMany(x => x.Ships).ToArray();

			// http://wikiwiki.jp/kancolle/?%C6%EE%C0%BE%BD%F4%C5%E7%B3%A4%B0%E8#search-calc
			// > 2-5式では説明出来ない事象を解決するため膨大な検証報告数より導き出した新式。2014年11月に改良され精度があがった。
			// > 索敵スコア
			// > = 艦上爆撃機 × (1.04) 
			// > + 艦上攻撃機 × (1.37)
			// > + 艦上偵察機 × (1.66)
			// > + 水上偵察機 × (2.00)
			// > + 水上爆撃機 × (1.78)
			// > + 小型電探 × (1.00)
			// > + 大型電探 × (.99)
			// > + 探照灯 × (0.91)
			// > + √(各艦毎の素索敵) × (1.69)
			// > + (司令部レベルを5の倍数に切り上げ) × (-0.61)

			var itemScore = ships
				.SelectMany(x => x.EquippedItems)
				.Select(x => x.Item.Info)
				.GroupBy(
					x => x.Type,
					x => x.RawData.api_saku,
					(type, scores) => new { type, score = scores.Sum() })
				.Aggregate(.0, (score, item) => score + GetScore(item.type, item.score));

			var shipScore = ships
				.Select(x => x.ViewRange - x.EquippedItems.Sum(s => s.Item.Info.RawData.api_saku))
				.Select(x => Math.Sqrt(x))
				.Sum() * 1.69;

			var level = (((KanColleClient.Current.Homeport.Admiral.Level + 4) / 5) * 5);
			var admiralScore = level * -0.61;

			return itemScore + shipScore + admiralScore;
		}

		private static double GetScore(SlotItemType type, int score)
		{
			switch (type)
			{
				case SlotItemType.艦上爆撃機:
					return score * 1.04;
				case SlotItemType.艦上攻撃機:
					return score * 1.37;
				case SlotItemType.艦上偵察機:
					return score * 1.66;

				case SlotItemType.水上偵察機:
					return score * 2.00;
				case SlotItemType.水上爆撃機:
					return score * 1.78;

				case SlotItemType.小型電探:
					return score * 1.00;
				case SlotItemType.大型電探:
					return score * .99;

				case SlotItemType.探照灯:
					return score * 0.91;
			}

			return .0;
		}
	}


	public class ViewRangeType4 : ViewRangeCalcLogic
	{
		public override sealed string Id => "KanColleViewer.Type4";

		public override string Name => "33 式";

		public override string Description =>
			@"((各スロットの装備の索敵値 + 改修効果) × 装備タイプ係数)の和 + (√各艦の素索敵値)の和
- (司令部レベル × 0.4)の小数点以下切り上げ + 艦隊の空き数 × 2
※艦隊の空き数は退避した艦を除いて算出";

		public override bool HasCombinedSettings { get; } = true;

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;

			var ships = this.GetTargetShips(fleets)
						.Where(x => !x.Situation.HasFlag(ShipSituation.Evacuation))
						.Where(x => !x.Situation.HasFlag(ShipSituation.Tow))
						.ToArray();

			if (!ships.Any()) return 0;

			var itemScore = ships
				.SelectMany(x => x.EquippedItems)
				.Select(x => x.Item)
				.Sum(x => (x.Info.ViewRange + GetAdeptCoefficient(x)) * GetTypeCoefficient(x.Info.Type));

			var shipScore = ships
				.Select(x => x.ViewRange - x.EquippedItems.Sum(s => s.Item.Info.RawData.api_saku))
				.Sum(x => Math.Sqrt(x));

			var admiralScore = Math.Ceiling(KanColleClient.Current.Homeport.Admiral.Level * 0.4);

			var isCombined = 1 < fleets.Count()
							 && KanColleClient.Current.Settings.IsViewRangeCalcIncludeFirstFleet
							 && KanColleClient.Current.Settings.IsViewRangeCalcIncludeSecondFleet;
			var vacancyScore = ((isCombined ? 12 : 6) - ships.Length) * 2;

			return itemScore + shipScore - admiralScore + vacancyScore;
		}

		private Ship[] GetTargetShips(Fleet[] fleets)
		{
			if (fleets.Count() == 1)
				return fleets.Single().Ships;

			if(KanColleClient.Current.Settings.IsViewRangeCalcIncludeFirstFleet
			&& KanColleClient.Current.Settings.IsViewRangeCalcIncludeSecondFleet)
				return fleets.SelectMany(x => x.Ships).ToArray();

			if (KanColleClient.Current.Settings.IsViewRangeCalcIncludeFirstFleet)
				return fleets.First().Ships;

			if (KanColleClient.Current.Settings.IsViewRangeCalcIncludeSecondFleet)
				return fleets.Last().Ships;

			return new Ship[0];
		}

		private static double GetAdeptCoefficient(SlotItem item)
		{
			switch (item.Info.Type)
			{
				case SlotItemType.水上偵察機:
					return Math.Sqrt(item.Adept) * 1.2;

				case SlotItemType.小型電探:
				case SlotItemType.大型電探:
				case SlotItemType.大型電探_II:
					return Math.Sqrt(item.Adept) * 1.25;

				default:
					return 0;
			}
		}

		private static double GetTypeCoefficient(SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.艦上戦闘機:
				case SlotItemType.艦上爆撃機:
				case SlotItemType.小型電探:
				case SlotItemType.大型電探:
				case SlotItemType.対潜哨戒機:
				case SlotItemType.探照灯:
				case SlotItemType.司令部施設:
				case SlotItemType.航空要員:
				case SlotItemType.水上艦要員:
				case SlotItemType.大型ソナー:
				case SlotItemType.大型飛行艇:
				case SlotItemType.大型探照灯:
				case SlotItemType.水上戦闘機:
					return 0.6;

				case SlotItemType.艦上攻撃機:
					return 0.8;

				case SlotItemType.艦上偵察機:
				case SlotItemType.艦上偵察機_II:
					return 1.0;

				case SlotItemType.水上爆撃機:
					return 1.1;

				case SlotItemType.水上偵察機:
					return 1.2;

				default:
					return .0;
			}
		}
	}
}
