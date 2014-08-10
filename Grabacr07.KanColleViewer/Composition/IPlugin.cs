using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// KanColleViewer プラグインの機能を公開します。
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// プラグインの設定画面を取得します。
		/// </summary>
		/// <returns>プラグインの設定画面に表示する UI 要素。</returns>
		object GetSettingsView();
	}
}
