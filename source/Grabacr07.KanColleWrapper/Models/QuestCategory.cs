using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Annotations;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 任務のカテゴリを示す識別子を定義します。
	/// </summary>
	public enum QuestCategory
	{
		/// <summary>
		/// 編成任務。
		/// </summary>
		Composition = 1,

		/// <summary>
		/// 出撃任務。
		/// </summary>
		Sortie = 2,

		/// <summary>
		/// 演習任務。
		/// </summary>
		Practice = 3,

		/// <summary>
		/// 遠征任務。
		/// </summary>
		Expeditions = 4,

		/// <summary>
		/// 補給/入渠任務。
		/// </summary>
		Supply = 5,

		/// <summary>
		/// 工廠任務。
		/// </summary>
		Building = 6,

		/// <summary>
		/// 改装任務。
		/// </summary>
		Remodelling = 7,

		/// <summary>
		/// 出撃任務。
		/// </summary>
		[Dark("艦これ API の規約 (？) で、任務では api_no の 3 桁目と api_category を一致させる実装になっている。")]
		[Dark("api_no において出撃任務だった 200 番台が溢れたためか、同じ出撃任務なのに 800 番台が登場し、出撃を示す api_category は 2 と 8 の 2 種ができてしまった。")]
		[Dark("クライアント処理で api_no の 3 桁目と api_category を照合するような処理は特に見受けられないので、普通に api_category = 2 で 800 番台の任務にすりゃいいのに。。。")]
		Sortie2 = 8,

		/// <summary>
		/// その他。
		/// </summary>
		Other = 9,
	}
}
