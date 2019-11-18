// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models
{
	public enum SlotItemType
	{
		小口径主砲 = 1,
		中口径主砲 = 2,
		大口径主砲 = 3,
		副砲 = 4,
		魚雷 = 5,
		艦上戦闘機 = 6,
		艦上爆撃機 = 7,
		艦上攻撃機 = 8,
		艦上偵察機 = 9,
		水上偵察機 = 10,
		水上爆撃機 = 11,
		小型電探 = 12,
		大型電探 = 13,
		ソナー = 14,
		爆雷 = 15,
		追加装甲 = 16,
		機関部強化 = 17,
		対空強化弾 = 18,
		対艦強化弾 = 19,
		VT信管 = 20,
		対空機銃 = 21,
		特殊潜航艇 = 22,
		応急修理要員 = 23,
		上陸用舟艇 = 24,
		オートジャイロ = 25,
		対潜哨戒機 = 26,
		追加装甲_中型 = 27,
		追加装甲_大型 = 28,
		探照灯 = 29,
		簡易輸送部材 = 30,
		艦艇修理施設 = 31,
		潜水艦魚雷 = 32,
		照明弾 = 33,
		司令部施設 = 34,
		航空要員 = 35,
		高射装置 = 36,
		対地装備 = 37,
		大口径主砲_II = 38,
		水上艦要員 = 39,
		大型ソナー = 40,
		大型飛行艇 = 41,
		大型探照灯 = 42,
		戦闘糧食 = 43,
		補給物資 = 44,
		水上戦闘機 = 45,
		特型内火艇 = 46,
		陸上攻撃機 = 47,
		局地戦闘機 = 48,
		陸上偵察機 = 49,
		輸送機材 = 50,
		潜水艦装備 = 51,
		噴式戦闘機 = 56,
		噴式戦闘爆撃機 = 57,
		噴式攻撃機 = 58,
		噴式偵察機 = 59,
		大型電探_II = 93,
		艦上偵察機_II = 94,
	}

	public static class SlotItemTypeExtensions
	{
		public static bool IsNumerable(this SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.艦上偵察機:
				case SlotItemType.艦上偵察機_II:
				case SlotItemType.艦上戦闘機:
				case SlotItemType.艦上攻撃機:
				case SlotItemType.艦上爆撃機:
				case SlotItemType.水上偵察機:
				case SlotItemType.水上爆撃機:
				case SlotItemType.水上戦闘機:
				case SlotItemType.オートジャイロ:
				case SlotItemType.対潜哨戒機:
				case SlotItemType.大型飛行艇:
				case SlotItemType.噴式戦闘機:
				case SlotItemType.噴式戦闘爆撃機:
				case SlotItemType.噴式攻撃機:
				case SlotItemType.噴式偵察機:
					return true;

				default:
					return false;
			}
		}
	}
}
