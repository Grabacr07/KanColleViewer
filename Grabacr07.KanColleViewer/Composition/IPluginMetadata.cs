using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// KanColleViewer プラグインのメタデータを公開します。
	/// </summary>
	public interface IPluginMetadata
	{
		/// <summary>
		/// プラグインのタイトルを取得します。
		/// </summary>
		string Title { get; }

		/// <summary>
		/// プラグインが提供する機能を簡潔に説明するテキストを取得します。
		/// </summary>
		string Description { get; }

		/// <summary>
		/// プラグインのバージョンを取得します。
		/// </summary>
		string Version { get; }

		/// <summary>
		/// プラグインの開発者を取得します。
		/// </summary>
		string Author { get; }
	}
}