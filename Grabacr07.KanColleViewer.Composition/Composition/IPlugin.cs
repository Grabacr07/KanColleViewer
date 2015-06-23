using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// KanColleViewer プラグインを表します。プラグインは、必ずこのコントラクト型でエクスポートしてください。
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// プラグインの初期化処理を実行します。
		/// </summary>
		void Initialize();
	}

	/// <summary>
	/// プラグイン側から本体に通知を要求するためのメンバーを公開します。
	/// </summary>
	public interface IRequestNotify
	{
		/// <summary>
		/// このプラグインが、本体に通知を要求したときに発生します。
		/// </summary>
		event EventHandler<NotifyEventArgs> NotifyRequested;
	}

	/// <summary>
	/// プラグインの設定画面を呼び出すためのメンバーを公開します。
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// [設定] タブ内に表示されるプラグイン設定 UI のルート要素を取得します。
		/// </summary>
		object View { get; }
	}

	/// <summary>
	/// [ツール] タブに表示されるツールに必要なメンバーを公開します。
	/// </summary>
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
