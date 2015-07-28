using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// 通知機能を公開します。
	/// このインターフェイスは、KanColleViewer プラグインのコントラクト型です。
	/// </summary>
	[PluginFeature]
	public interface INotifier
	{
		void Notify(INotification notification);
	}
}
