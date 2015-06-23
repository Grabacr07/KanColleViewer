using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// [ツール] タブに表示されるツールに必要なメンバーを公開します。
	/// </summary>
	public interface ITool
	{
		/// <summary>
		/// ツール名を取得します。
		/// </summary>
		string Name { get; }

		/// <summary>
		/// [ツール] タブに表示する UI のルート要素を取得します。
		/// </summary>
		object View { get; }
	}
}
