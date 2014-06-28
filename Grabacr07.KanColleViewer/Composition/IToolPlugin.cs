using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグインの主機能を [ツール] タブに表示させるためのメンバーを公開します。
	/// </summary>
	public interface IToolPlugin : IPlugin
	{
		string ToolName { get; }

		object GetToolView();
	}
}
