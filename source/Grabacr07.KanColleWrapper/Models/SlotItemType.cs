// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models
{
	public enum SlotItemType
	{
		None = 0,
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
				case SlotItemType.陸上攻撃機:
				case SlotItemType.局地戦闘機:
				case SlotItemType.噴式戦闘機:
				case SlotItemType.噴式戦闘爆撃機:
				case SlotItemType.噴式攻撃機:
				case SlotItemType.噴式偵察機:
					return true;

				default:
					return false;
			}
		}
		public static string GetName(this SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.None:
					return "없음";

				case SlotItemType.小口径主砲:
					return "소구경주포";
				case SlotItemType.中口径主砲:
					return "중구경주포";
				case SlotItemType.大口径主砲:
					return "대구경주포";
				case SlotItemType.副砲:
					return "부포";
				case SlotItemType.魚雷:
					return "어뢰";
				case SlotItemType.艦上戦闘機:
					return "함상전투기";
				case SlotItemType.艦上爆撃機:
					return "함상폭격기";
				case SlotItemType.艦上攻撃機:
					return "함상공격기";
				case SlotItemType.艦上偵察機:
					return "함상정찰기";
				case SlotItemType.水上偵察機:
					return "수상정찰기";
				case SlotItemType.水上爆撃機:
					return "수상폭격기";
				case SlotItemType.小型電探:
					return "소형전탐";
				case SlotItemType.大型電探:
					return "대형전탐";
				case SlotItemType.ソナー:
					return "소나";
				case SlotItemType.爆雷:
					return "폭뢰";
				case SlotItemType.追加装甲:
					return "추가장갑";
				case SlotItemType.機関部強化:
					return "기관부강화";
				case SlotItemType.対空強化弾:
					return "대공강화탄";
				case SlotItemType.対艦強化弾:
					return "대함강화탄";
				case SlotItemType.VT信管:
					return "VT신관";
				case SlotItemType.対空機銃:
					return "대공기관총";
				case SlotItemType.特殊潜航艇:
					return "특수잠항정";
				case SlotItemType.応急修理要員:
					return "응급수리요원";
				case SlotItemType.上陸用舟艇:
					return "상륙용주정";
				case SlotItemType.オートジャイロ:
					return "오토자이로";
				case SlotItemType.対潜哨戒機:
					return "대잠초계기";
				case SlotItemType.追加装甲_中型:
					return "추가장갑 (중형)";
				case SlotItemType.追加装甲_大型:
					return "추가장갑 (대형)";
				case SlotItemType.探照灯:
					return "탐조등";
				case SlotItemType.簡易輸送部材:
					return "간이운송자재";
				case SlotItemType.艦艇修理施設:
					return "함정수리시설";
				case SlotItemType.潜水艦魚雷:
					return "잠수함어뢰";
				case SlotItemType.照明弾:
					return "조명탄";
				case SlotItemType.司令部施設:
					return "사령부시설";
				case SlotItemType.航空要員:
					return "항공요원";
				case SlotItemType.高射装置:
					return "고사장치";
				case SlotItemType.対地装備:
					return "대지상장비";
				case SlotItemType.大口径主砲_II:
					return "대구경주포 2";
				case SlotItemType.水上艦要員:
					return "숙련견시원";
				case SlotItemType.大型ソナー:
					return "대형소나";
				case SlotItemType.大型飛行艇:
					return "대형비행정";
				case SlotItemType.大型探照灯:
					return "대형탐조등";
				case SlotItemType.戦闘糧食:
					return "전투식량";
				case SlotItemType.補給物資:
					return "보급물자";
				case SlotItemType.水上戦闘機:
					return "수상전투기";
				case SlotItemType.特型内火艇:
					return "특형내화정";
				case SlotItemType.陸上攻撃機:
					return "육상공격기";
				case SlotItemType.局地戦闘機:
					return "국지전투기";
				case SlotItemType.輸送機材:
					return "운송자재";
				case SlotItemType.潜水艦装備:
					return "잠수함장비";
				case SlotItemType.噴式戦闘機:
					return "분식 전투기";
				case SlotItemType.噴式戦闘爆撃機:
					return "분식 전투폭격기";
				case SlotItemType.噴式攻撃機:
					return "분식 공격기";
				case SlotItemType.噴式偵察機:
					return "분식 정찰기";
				case SlotItemType.大型電探_II:
					return "대형전탐 2";
				case SlotItemType.艦上偵察機_II:
					return "함상정찰기 2";

				default:
					return type.ToString();
			}
		}
	}
}
