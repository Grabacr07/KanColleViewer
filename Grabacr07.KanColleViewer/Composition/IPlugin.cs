using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public interface IPlugin
	{
		/// <summary>
		/// プラグインの設定画面を取得します。
		/// </summary>
		/// <returns></returns>
		object GetSettingsView();
	}

	public interface IPluginMetadata
	{
		string Name { get; }

		[DefaultValue("1.0")]
		string Version { get; }

		string Author { get; }
	}
}
