using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// [ツール] タブに表示されるツールに必要なメンバーを公開します。
	/// このインターフェイスは、KanColleViewer プラグインのコントラクト型です。
	/// </summary>
	[PluginFeature]
	public interface ITool
	{
		/// <summary>
		/// [ツール] タブのツール一覧に表示される名前を取得します。
		/// </summary>
		string Name { get; }

		/// <summary>
		/// [ツール] タブ内に表示される UI のルート要素を取得します。
		/// </summary>
		object View { get; }
	}
}
