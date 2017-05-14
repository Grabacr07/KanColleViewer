using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	internal class ASWCalculator
	{
		public static string GetASWTooltip(Ship ship)
		{
			//  4식 소나 대잠 12
			//  0식 소나 대잠 11
			//  3식 소나 대잠 10
			//  3식 폭뢰 대잠  8
			//  2식 폭뢰 대잠  7
			// 93식 소나 대잠  6
			// 94식 폭뢰 대잠  5
			// 95식 폭뢰 대잠  4

			// 선제 대잠에는 소나 장착이 필수

			List<string> output = new List<string>();
			var master = KanColleClient.Current.Master;

			// 선제 대잠에 필요한 대잠 수치
			var goal_asw = ship.Info.ShipType.Id == 1 ? 60
				: ship.Info.ShipType.Id == 7 && ship.Speed == ShipSpeed.Slow ? 65
				: 100;

			// 필요한 장비 대잠 수치
			var require_asw = Math.Max(0, goal_asw - ship.ASW.Current);

			IEnumerable<SlotItemInfo> available_equips;
			List<SlotItemType> available_equip_types = new List<SlotItemType>();
			SlotItemType[] require_type = new SlotItemType[0];

			#region 장착 가능 & 대잠 공격 필수 장비 체크
			switch (ship.Info.ShipType.Id)
			{
				case 1:  // 해방함
				case 2:  // 구축함
				case 3:  // 경순양함
				case 4:  // 중뢰장순양함
				case 21: // 연습순양함
					available_equip_types.Add(SlotItemType.ソナー); // 소나
					available_equip_types.Add(SlotItemType.爆雷); // 폭뢰

					require_type = new SlotItemType[] { SlotItemType.ソナー };
					break;

				case 6:  // 항공순양함
				case 10: // 항공전함
					available_equip_types.Add(SlotItemType.水上爆撃機); // 수상폭격기
					available_equip_types.Add(SlotItemType.大型ソナー); // 대형소나
					available_equip_types.Add(SlotItemType.オートジャイロ); // 오토자이로

					require_type = new SlotItemType[] { SlotItemType.水上爆撃機 };
					break;

				case 16: // 수상기모함
					available_equip_types.Add(SlotItemType.水上爆撃機); // 수상폭격기
					available_equip_types.Add(SlotItemType.大型ソナー); // 대형소나

					if (ship.Info.Id == 450 // 까모改
						|| ship.Info.Id == 491 // 코망단
						|| ship.Info.Id == 372) // 코망단改
						available_equip_types.Add(SlotItemType.ソナー); // 소나

					if (ship.Info.Id != 491 // 코망단
						&& ship.Info.Id != 372) // 코망단改
						available_equip_types.Add(SlotItemType.爆雷); // 폭뢰

					if (ship.Info.Id == 491 // 코망단
						|| ship.Info.Id == 372) // 코망단改
						available_equip_types.Add(SlotItemType.オートジャイロ); // 오토자이로

					require_type = new SlotItemType[] { SlotItemType.水上爆撃機, SlotItemType.オートジャイロ };
					break;

				case 7:  // 경공모
					available_equip_types.Add(SlotItemType.艦上爆撃機); // 함상폭격기

					if (ship.Info.Id != 521) // 카스가마루
					{
						available_equip_types.Add(SlotItemType.艦上攻撃機); // 함상공격기

						available_equip_types.Add(SlotItemType.オートジャイロ); // 오토자이로
						available_equip_types.Add(SlotItemType.対潜哨戒機); // 대잠초계기
					}

					available_equip_types.Add(SlotItemType.大型ソナー); // 대형소나
					if (ship.Info.Id == 380 // 타이요改
						|| ship.Info.Id == 529) // 타이요改2
					{
						available_equip_types.Add(SlotItemType.ソナー); // 소나
						available_equip_types.Add(SlotItemType.爆雷); // 폭뢰
					}

					require_type = new SlotItemType[] { SlotItemType.艦上爆撃機, SlotItemType.艦上攻撃機 };
					break;

				case 22: // 보급함
					if (ship.Info.Id == 352)// 하야스이改
					{
						available_equip_types.Add(SlotItemType.艦上攻撃機); // 함상공격기
						available_equip_types.Add(SlotItemType.ソナー); // 소나
					}
					available_equip_types.Add(SlotItemType.オートジャイロ); // 오토자이로

					require_type = new SlotItemType[] { SlotItemType.艦上攻撃機, SlotItemType.オートジャイロ };
					break;

				case 17: // 양륙함
					available_equip_types.Add(SlotItemType.大型ソナー); // 대형소나
					available_equip_types.Add(SlotItemType.オートジャイロ); // 오토자이로
					available_equip_types.Add(SlotItemType.対潜哨戒機); // 대잠초계기

					require_type = new SlotItemType[] { SlotItemType.オートジャイロ, SlotItemType.対潜哨戒機 };
					break;
			}
			#endregion

			// 산출된 목록에서 실제 장비를 가져옴
			available_equips = master.SlotItems
				.Where(x => x.Value.Id <= 500) // 501 부터는 심해서함 장비
				.Where(x => available_equip_types.Contains(x.Value.Type))
				.Select(x => x.Value)
				.ToArray();
			require_type = require_type
				.Where(x => available_equip_types.Contains(x))
				.ToArray();

			// 장착 가능 장비가 없는 경우 혹은 필수 장비가 항공기인데 탑재량이 없는 경우
			if (available_equips.Count() == 0 || require_type.Count() == 0
				|| (require_type.All(x => x.IsNumerable()) && ship.Info.Slots.All(x => x == 0)))
			{
				output.Add("선제 대잠 불가능");
				output.Add("- 대잠 공격 불가능");
			}
			else if (ship.Info.Id == 141) // 이스즈
			{
				output.Add("선제 대잠 가능");
				output.Add("- 대잠 공격 필수 장비:");
				for (var i = 0; i < require_type.Length; i++)
					output.Add($"  * {require_type[i].GetName()}");

				goal_asw = require_asw = 0;
			}
			else
			{
				// 대잠 낮은 장비순으로 (높은 순서지만 Stack 에서 Pop 되므로 거꾸로 해야한다)
				var sorted_items = available_equips
					.OrderByDescending(x => x.ASW)
					.ToArray();

				// 장착 가능한 슬롯 수 만큼
				Stack<SlotItemInfo>[] available_items = new Stack<SlotItemInfo>[ship.Info.SlotCount];
				for (var i = 1; i < ship.Info.SlotCount; i++)
					available_items[i] = new Stack<SlotItemInfo>(sorted_items);

				// 1슬롯은 필수 장비를
				available_items[0] = new Stack<SlotItemInfo>(
					available_equips
						.Where(x => require_type.Contains(x.Type))
						.OrderByDescending(x => x.ASW)
				);

				// 가능한 최소 장비 세트를 찾기
				var last_asw = 0;
				while (available_items[0].Count > 0)
				{
					var cur_asw = available_items.Sum(x => x.Peek().ASW);
					last_asw = cur_asw;

					if (cur_asw >= require_asw)
					{
						output.Add("선제 대잠 가능");
						output.Add("- 대잠 공격 필수 장비:");
						for (var i = 0; i < require_type.Length; i++)
							output.Add($"  * {require_type[i].GetName()}");

						output.Add("- 최소 장비:");
						for (var i = 0; i < available_items.Length; i++)
							output.Add($"  * {available_items[i].Peek().Name}");

						break;
					}
					else
					{
						for (var i = available_items.Length - 1; i >= 0; i--)
						{
							available_items[i].Pop();

							if (available_items[i].Count == 0 && i != 0)
								available_items[i] = new Stack<SlotItemInfo>(sorted_items);
							else
								break;
						}
					}
				}

				// 찾지 못한 경우
				if (available_items[0].Count == 0)
				{
					output.Add("선제 대잠 불가능");
					output.Add("- 대잠 공격 필수 장비:");
					for (var i = 0; i < require_type.Length; i++)
						output.Add($"  * {require_type[i].GetName()}");
					output.Add("- 대잠 수치 부족");
					output.Add($"- 함선 대잠 수치 {require_asw - last_asw} 증가 필요");
				}
			}

			output.Add("");
			output.Add("선제 대잠에 필요한 대잠 수치");
			output.Add(" - 필요 대잠 수치");
			output.Add($"  * {goal_asw}");
			output.Add(" - 필요 장비 대잠 수치 합계");
			output.Add($"  * {require_asw}");

			return string.Join(Environment.NewLine, output.ToArray());
		}
	}
}
