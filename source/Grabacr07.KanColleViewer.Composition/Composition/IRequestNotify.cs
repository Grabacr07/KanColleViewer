using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグイン側から本体に通知を要求するためのメンバーを公開します。
	/// このインターフェイスは、KanColleViewer プラグインのコントラクト型です。
	/// </summary>
	[PluginFeature]
	public interface IRequestNotify
	{
		/// <summary>
		/// このプラグインが、本体に通知を要求したときに発生します。
		/// </summary>
		event EventHandler<NotifyEventArgs> NotifyRequested;
	}
}
