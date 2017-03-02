using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/* Comment
	 * * * * * * * * * * * * * * *
	 * 칸코레 갤러리 개념글 및 Akashi-list.me, 그 외 일본 블로그 등의 자료를 참고하여 데이터를 사용중
	 * 구축함에 대한 피트 보정이 있다는 이야기는 없음
	 * 중순양함은 함급 구분 없이 전체가 동일사양인 듯 함
	 */

	/// <summary>
	/// <see cref="ShipSlot"/>에서 주포 피트 계산을 위한 함급 구분
	/// </summary>
	public enum ShipFitClass
	{
		/// <summary>
		/// Not Applicable
		/// </summary>
		NA,

		#region 전함
		/// <summary>
		/// 야마토급 (大和, 武蔵)
		/// </summary>
		YamatoClass,

		/// <summary>
		/// 나가토급 (長門, 陸奥)
		/// </summary>
		NagatoClass,

		/// <summary>
		/// 이세급 (伊勢, 日向)
		/// </summary>
		IseClass,

		/// <summary>
		/// 후소급 (扶桑, 山城)
		/// </summary>
		FusouClass,

		/// <summary>
		/// V.Veneto급 (Littorio-Italia, Roma)
		/// </summary>
		V_VenetoClass,

		/// <summary>
		/// Bismarck급 (Bismarck)
		/// </summary>
		BismarckClass,

		/// <summary>
		/// 콩고급 (金剛, 比叡, 榛名, 霧島)
		/// </summary>
		KongouClass,

		/// <summary>
		/// Iowa급 (Iowa)
		/// </summary>
		IowaClass,

		/// <summary>
		/// Queen Elizabeth급 (Warspite)
		/// </summary>
		QueenElizabethClass,
		#endregion

		#region 경순양함, 중뇌장순양함, 연습순양함
		/// <summary>
		/// 센다이급 (川内, 神通, 那珂)
		/// </summary>
		SendaiClass,

		/// <summary>
		/// 텐류급 (天龍, 龍田)
		/// </summary>
		TenryuuClass,

		/// <summary>
		/// 쿠마급 (球磨, 多摩)
		/// </summary>
		KumaClass,

		/// <summary>
		/// 나가라급 (長良, 五十鈴, 名取, 由良, 鬼怒, 阿武隈)
		/// </summary>
		NagaraClass,

		/// <summary>
		/// 유바리급 (夕張)
		/// </summary>
		YuubariClass,

		/// <summary>
		/// 오요도급 (大淀)
		/// </summary>
		OyodoClass,

		/// <summary>
		/// 아가노급 (阿賀野, 能代, 矢矧, 酒匂)
		/// </summary>
		AganoClass,

		/// <summary>
		/// 쿠마급 중뇌장순양함 (北上, 大井, 木曾)
		/// </summary>
		KumaTorpedoClass,

		/// <summary>
		/// 카토리급 (香取, 鹿島)
		/// </summary>
		KatoriClass,
		#endregion

		#region 중순양함
		/// <summary>
		/// 중순양함
		/// </summary>
		HeavyCruiser,
		#endregion
	}

	public class ShipFitClassUtil
	{
		public static Dictionary<int, Dictionary<ShipFitClass, int>> FitTable_Heavy { get; }
		public static Dictionary<int, Dictionary<ShipFitClass, int>> FitTable_Medium { get; }

		static ShipFitClassUtil()
		{
			FitTable_Heavy = new Dictionary<int, Dictionary<ShipFitClass, int>>();
			FitTable_Medium = new Dictionary<int, Dictionary<ShipFitClass, int>>();

			#region 대구경주포 피트 테이블
			/* Comment - 대구경주포
			 * * * * * * * * * * * * * * *
			 * 381mm/50 삼연장포        : 전체 데이터 불명, 381mm/50 삼연장포改 데이터를 임시로 사용
			 * 38cm연장포               : 전체 데이터 불명, 38cm연장포改 데이터를 임시로 사용
			 * 35.6연장포(다즐미채)     : 이세급 제외 데이터 불명, 35.6연장포 데이터를 임시로 사용
			 * 16inch삼연장포 Mk.7+GFCS : 전체 데이터 불명, 16inch삼연장포 Mk.7 데이터를 임시로 사용
			 * 38.1cm Mk.I연장포        : 콩고급, V.Veneto, 퀸 엘리자베스급 제외 데이터 불명
			 * 38.1cm Mk.I/N연장포改    : 전체 데이터 불명, 38.1cm Mk.I연장포 데이터를 임시로 사용
			 */

			FitTable_Heavy.Add(128, ShipFitClassUtil.ParseHeavy(" 0, 0,  ,  ,  ,  ,  ,  ,  ")); // 試製51cm連装砲

			FitTable_Heavy.Add(9,   ShipFitClassUtil.ParseHeavy(" 0,-1,-2,-2,-2,-2,-2,-2,-2")); // 46cm三連装砲
			FitTable_Heavy.Add(117, ShipFitClassUtil.ParseHeavy(" 0,-1,-1,-1,-1,-1,-1,-1,-1")); // 試製46cm連装砲

			FitTable_Heavy.Add(8,   ShipFitClassUtil.ParseHeavy(" 0, 0, 0,+1,-1,-1,-1,-1, 0")); // 41cm連装砲
			FitTable_Heavy.Add(105, ShipFitClassUtil.ParseHeavy(" 0, 0, 0,+1,-1,-1,-1,-1, 0")); // 試製41cm三連装砲

			// FitTable_Heavy.Add(133, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, ?, ?, ?, ?, ?")); // 381mm/50 三連装砲
			FitTable_Heavy.Add(133, ShipFitClassUtil.ParseHeavy(" 0,+1, 0,+1, 0, 0, 0, 0,-1")); // 381mm/50 三連装砲
			FitTable_Heavy.Add(137, ShipFitClassUtil.ParseHeavy(" 0,+1, 0,+1, 0, 0, 0, 0,-1")); // 381mm/50 三連装砲改

			// FitTable_Heavy.Add(76, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, ?, ?, ?, ?, ?")); // 38cm連装砲
			FitTable_Heavy.Add(76,  ShipFitClassUtil.ParseHeavy(" 0, 0,+1,+1,+1,+1,+2,+1,+1")); // 38cm連装砲
			FitTable_Heavy.Add(114, ShipFitClassUtil.ParseHeavy(" 0, 0,+1,+1,+1,+1,+2,+1,+1")); // 38cm連装砲改

			FitTable_Heavy.Add(7,   ShipFitClassUtil.ParseHeavy(" 0, 0,+1,+1,+1,+1,+2, 0,+1")); // 35.6cm連装砲
			FitTable_Heavy.Add(103, ShipFitClassUtil.ParseHeavy(" 0, 0,+1,+1,+1,+1,+2,+1,+1")); // 試製35.6cm三連装砲
			// FitTable_Heavy.Add(104, ShipFitClassUtil.ParseHeavy(" 0, ?, ?, ?, ?, ?, ?, ?, ?")); // 35.6cm連装砲(ダズル迷彩)
			FitTable_Heavy.Add(104, ShipFitClassUtil.ParseHeavy(" 0, 0,+1,+1,+1,+1,+2, 0,+1")); // 35.6cm連装砲(ダズル迷彩)

			FitTable_Heavy.Add(161, ShipFitClassUtil.ParseHeavy(" 0, 0,-1, 0,-1,-1,-1, 0,-1")); // 16inch三連装砲 Mk.7
			// FitTable_Heavy.Add(183, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, ?, ?, ?, ?, ?")); // 16inch三連装砲 Mk.7+GFCS
			FitTable_Heavy.Add(183, ShipFitClassUtil.ParseHeavy(" 0, 0,-1, 0,-1,-1,-1, 0,-1")); // 16inch三連装砲 Mk.7+GFCS

			FitTable_Heavy.Add(190, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, 0, ?,+1, ?, 0")); // 38.1cm Mk.I連装砲
			// FitTable_Heavy.Add(192, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, ?, ?, ?, ?, ?")); // 38.1cm Mk.I/N連装砲改
			FitTable_Heavy.Add(192, ShipFitClassUtil.ParseHeavy(" ?, ?, ?, ?, 0, ?,+1, ?, 0")); // 38.1cm Mk.I/N連装砲改

			// FitTable_Heavy.Add(0,   ShipFitClassUtil.ParseHeavy("- ,- ,- ,- ,- ,- ,- ,- ,- ")); // NAME
			#endregion

			#region 중구경주포/부포 피트 테이블
			/* Comment - 중구경주포/부포
			 * * * * * * * * * * * * * * *
			 * 중순양함은 함급 구분 없이 공통적으로 피트가 적용된다고 가정
			 * 아직까지는 20.3cm 연장포, 3호포만 피트 보정이 있다는 이야기
			 * 2호포 및 해외 20.3cm/203mm에 과적이 있다는 이야기는 없으므로 무보정으로 가정
			 * ---------------------------------------------------------------------------------------
			 * 쿠마급은 쿠마, 타마만 선택, 중뇌장순양함은 개장 전의 키타카미, 오오이, 키소 전체 선택함
			 * 텐류급 및 나가라급이 없어서 평균 데이터인 유바리/오요도/아가노급/센다이급 데이터에 맞춤
			 * 20.3 시리즈가 일제 주포 20.3cm 인지, 해외 20.3cm/203mm 도 포함인지 모르지만 일단 포함
			 * 14cm단장포/연장포 및 15.2cm 부포도 피트에 포함된다는 일본 블로그 정보 - http://rankasan.hatenadiary.com/entry/2016/06/13/163132
			 * 15.2cm 주포 피트에 15.2cm 부포 피트이므로 (동구경이므로), 15.5cm 부포도 노패널티로 가정
			 * OTO 152mm가 해외 부포여서 보정이 없다고 가정하고, 15cm 연장부포 또한 무보정으로 가정
			 */

			FitTable_Medium.Add(6,   ShipFitClassUtil.ParseMedium("-1,0 ,-1,-1,-1,-1,-1,-1,+1")); // 20.3cm連装砲
			FitTable_Medium.Add(90,  ShipFitClassUtil.ParseMedium("-1,0 ,-1,-1,-1,-1,-1,-1, 0")); // 20.3cm(2号)連装砲
			FitTable_Medium.Add(50,  ShipFitClassUtil.ParseMedium("-1,0 ,-1,-1,-1,-1,-1,-1,+1")); // 20.3cm(3号)連装砲
			FitTable_Medium.Add(123, ShipFitClassUtil.ParseMedium("-1,0 ,-1,-1,-1,-1,-1,-1, 0")); // SKC34 20.3cm連装砲
			FitTable_Medium.Add(162, ShipFitClassUtil.ParseMedium("-1,0 ,-1,-1,-1,-1,-1,-1, 0")); // 203mm/53 連装砲

			FitTable_Medium.Add(5,   ShipFitClassUtil.ParseMedium(" 0, 0, 0, 0, 0, 0, 0, 0, 0")); // 15.5cm三連装砲 (主砲)
			FitTable_Medium.Add(12,  ShipFitClassUtil.ParseMedium(" 0, 0, 0, 0, 0, 0, 0, 0, 0")); // 15.5cm三連装副砲 (副砲)

			FitTable_Medium.Add(4,   ShipFitClassUtil.ParseMedium("+1,+1,+1,+1,+1,+1, 0,+1, 0")); // 14cm単装砲
			FitTable_Medium.Add(119, ShipFitClassUtil.ParseMedium("+1,+1,+1,+1,+1,+1, 0,+1, 0")); // 14cm連装砲
			FitTable_Medium.Add(11,  ShipFitClassUtil.ParseMedium("+1,+1,+1,+1,+1,+1, 0,+1, 0")); // 15.2cm単装砲 (副砲)
			FitTable_Medium.Add(65,  ShipFitClassUtil.ParseMedium("+1,+1,+1,+1,+1,+1, 0,+1, 0")); // 15.2cm連装砲
			FitTable_Medium.Add(139, ShipFitClassUtil.ParseMedium("+1,+1,+1,+1,+1,+1, 0,+1, 0")); // 15.2cm連装砲改

			FitTable_Medium.Add(77,  ShipFitClassUtil.ParseMedium(" 0, 0, 0, 0, 0, 0, 0, 0, 0")); // 15cm連装副砲
			FitTable_Medium.Add(134, ShipFitClassUtil.ParseMedium(" 0, 0, 0, 0, 0, 0, 0, 0, 0")); // OTO 152mm三連装速射砲

			// FitTable_Medium.Add(0,   ShipFitClassUtil.ParseMedium("- ,- ,- ,- ,- ,- ,- ,- ,- ,- ")); // NAME
			#endregion
		}

		/// <summary>
		/// 대구경 주포 피트 테이블 생성
		/// </summary>
		/// <param name="Table">야마토급, 나가토급, 이세급, 후소급, V.Veneto급, 비스마르크급, 콩고급, 아이오와급, 퀸 엘리자베스급</param>
		/// <returns>생성된 피트 테이블</returns>
		private static Dictionary<ShipFitClass, int> ParseHeavy(string Table)
		{
			string[] part = Table.Replace(" ", "").Replace("+", "")
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			int[] values = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			int len = Math.Min(values.Length, part.Length);
			for (var i = 0; i < len; i++)
				int.TryParse(part[i], out values[i]);

			return new Dictionary<ShipFitClass, int>
				{
					{ ShipFitClass.YamatoClass,         values[0] },
					{ ShipFitClass.NagatoClass,         values[1] },
					{ ShipFitClass.IseClass,            values[2] },
					{ ShipFitClass.FusouClass,          values[3] },
					{ ShipFitClass.V_VenetoClass,       values[4] },
					{ ShipFitClass.BismarckClass,       values[5] },
					{ ShipFitClass.KongouClass,         values[6] },
					{ ShipFitClass.IowaClass,           values[7] },
					{ ShipFitClass.QueenElizabethClass, values[8] },
				};
		}

		/// <summary>
		/// 중구경 주포/부포 피트 테이블 생성
		/// </summary>
		/// <param name="Table">센다이급, 쿠마급 경순, 텐류급, 나가라급, 유바리급&amp;오요도급, 아가노급, 쿠마급 중뇌순, 카토리급, 중순양함</param>
		/// <returns>생성된 피트 테이블</returns>
		private static Dictionary<ShipFitClass, int> ParseMedium(string Table)
		{
			string[] part = Table.Replace(" ", "").Replace("+", "")
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			int[] values = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0 ,0 ,0 };
			int len = Math.Min(values.Length, part.Length);
			for (var i = 0; i < len; i++)
				int.TryParse(part[i], out values[i]);

			return new Dictionary<ShipFitClass, int>
				{
					{ ShipFitClass.SendaiClass,      values[0] },
					{ ShipFitClass.KumaClass,        values[1] },
					{ ShipFitClass.TenryuuClass,     values[2] },
					{ ShipFitClass.NagaraClass,      values[3] },
					{ ShipFitClass.YuubariClass,     values[4] },
					{ ShipFitClass.OyodoClass,       values[5] },
					{ ShipFitClass.AganoClass,       values[6] },
					{ ShipFitClass.KumaTorpedoClass, values[7] },
					{ ShipFitClass.KatoriClass,      values[8] },
					{ ShipFitClass.HeavyCruiser,     values[9] },
				};
		}

		/// <summary>
		/// 칸무스 정보로 함급 분류
		/// </summary>
		/// <param name="shipInfo"></param>
		/// <returns></returns>
		public static ShipFitClass FromShipInfo(ShipInfo shipInfo)
		{
			if (shipInfo.ShipType.Id == 5 || shipInfo.ShipType.Id == 6) // 중순양함, 항공순양함
				return ShipFitClass.HeavyCruiser;

			switch (shipInfo.Id)
			{
				#region 전함
				case 131: // 大和
				case 136: // 大和改
				case 143: // 武蔵
				case 148: // 武蔵改
					return ShipFitClass.YamatoClass;

				case 80:  // 長門
				case 275: // 長門改
				case 81:  // 陸奥
				case 276: // 陸奥改
					return ShipFitClass.NagatoClass;

				case 77:  // 伊勢
				case 82:  // 伊勢改
				case 87:  // 日向
				case 88:  // 日向改
					return ShipFitClass.IseClass;

				case 26:  // 扶桑
				case 286: // 扶桑改
				case 411: // 扶桑改二
				case 27:  // 山城
				case 287: // 山城改
				case 412: // 山城改二
					return ShipFitClass.FusouClass;

				case 441: // Littorio
				case 446: // Italia
				case 442: // Roma
				case 447: // Roma改
					return ShipFitClass.V_VenetoClass;

				case 171: // Bismarck
				case 172: // Bismarck改
				case 173: // Bismarck zwei
				case 178: // Bismarck drei
					return ShipFitClass.BismarckClass;

				case 78:  // 金剛
				case 209: // 金剛改
				case 149: // 金剛改二
				case 86:  // 比叡
				case 210: // 比叡改
				case 150: // 比叡改二
				case 79:  // 榛名
				case 211: // 榛名改
				case 151: // 榛名改二
				case 85:  // 霧島
				case 212: // 霧島改
				case 152: // 霧島改二
					return ShipFitClass.KongouClass;

				case 440: // Iowa
				case 360: // Iowa改
					return ShipFitClass.IowaClass;

				case 439: // Warspite
				case 364: // Warspite改
					return ShipFitClass.QueenElizabethClass;
				#endregion

				#region 경순양함, 중뇌장순양함, 연습순양함
				case 54:  // 川内
				case 222: // 川内改
				case 158: // 川内改二
				case 55:  // 神通
				case 223: // 神通改
				case 159: // 神通改二
				case 56:  // 那珂
				case 224: // 那珂改
				case 160: // 那珂改二
					return ShipFitClass.SendaiClass;

				case 51:  // 天龍
				case 213: // 天龍改
				case 52:  // 龍田
				case 214: // 龍田改
					return ShipFitClass.TenryuuClass;

				case 99:  // 球磨
				case 215: // 球磨改
				case 100: // 多摩
				case 216: // 多摩改
					return ShipFitClass.KumaClass;

				case 21:  // 長良
				case 218: // 長良改
				case 22:  // 五十鈴
				case 219: // 五十鈴改
				case 141: // 五十鈴改二
				case 53:  // 名取
				case 221: // 名取改
				case 23:  // 由良
				case 220: // 由良改
				case 113: // 鬼怒
				case 289: // 鬼怒改
				case 487: // 鬼怒改二
				case 114: // 阿武隈
				case 290: // 阿武隈改
				case 200: // 阿武隈改二
					return ShipFitClass.NagaraClass;

				case 115: // 夕張
				case 293: // 夕張改
					return ShipFitClass.YuubariClass;

				case 183: // 大淀
				case 321: // 大淀改
					return ShipFitClass.OyodoClass;

				case 137: // 阿賀野
				case 305: // 阿賀野改
				case 138: // 能代
				case 306: // 能代改
				case 139: // 矢矧
				case 307: // 矢矧改
				case 140: // 酒匂
				case 314: // 酒匂改
					return ShipFitClass.AganoClass;

				case 25:  // 北上
				case 58:  // 北上改
				case 119: // 北上改二
				case 24:  // 大井
				case 57:  // 大井改
				case 118: // 大井改二
				case 101: // 木曾
				case 217: // 木曾改
				case 146: // 木曾改二
					return ShipFitClass.KumaTorpedoClass;

				case 154: // 香取
				case 343: // 香取改
				case 465: // 鹿島
				case 356: // 鹿島改
					return ShipFitClass.KatoriClass;
				#endregion
			}
			return ShipFitClass.NA;
		}
	}
}
