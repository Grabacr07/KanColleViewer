using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum SlotItemIconType
	{
		Unknown = 0,

		/// <summary>
		/// 小口径主砲。
		/// </summary>
		MainCanonLight = 1,

		/// <summary>
		/// 中口径主砲。
		/// </summary>
		MainCanonMedium = 2,

		/// <summary>
		/// 大口径主砲。
		/// </summary>
		MainCanonHeavy = 3,

		/// <summary>
		/// 副砲。
		/// </summary>
		SecondaryCanon = 4,

		/// <summary>
		/// 魚雷。
		/// </summary>
		Torpedo = 5,

		/// <summary>
		/// 艦戦。
		/// </summary>
		Fighter = 6,

		/// <summary>
		/// 艦爆。
		/// </summary>
		DiveBomber = 7,

		/// <summary>
		/// 艦攻。
		/// </summary>
		TorpedoBomber = 8,

		/// <summary>
		/// 偵察機。
		/// </summary>
		ReconPlane = 9,

		/// <summary>
		/// 水上機。
		/// </summary>
		ReconSeaplane = 10,

		/// <summary>
		/// 電探。
		/// </summary>
		Rader = 11,

		/// <summary>
		/// 三式弾。
		/// </summary>
		AAShell = 12,

		/// <summary>
		/// 徹甲弾。
		/// </summary>
		APShell = 13,

		/// <summary>
		/// ダメコン。
		/// </summary>
		DamageControl = 14,

		/// <summary>
		/// 機銃。
		/// </summary>
		AAGun = 15,

		/// <summary>
		/// 高角砲。
		/// </summary>
		HighAngleGun = 16,

		/// <summary>
		/// 爆雷投射機。
		/// </summary>
		ASW = 17,

		/// <summary>
		/// ソナー。
		/// </summary>
		Soner = 18,

		/// <summary>
		/// 機関部強化。
		/// </summary>
		EngineImprovement = 19,

		/// <summary>
		/// 上陸用舟艇。
		/// </summary>
		LandingCraft = 20,

		/// <summary>
		/// オートジャイロ。
		/// </summary>
		Autogyro = 21,

		/// <summary>
		/// 指揮連絡機。
		/// </summary>
		ArtillerySpotter = 22,
	}
}
