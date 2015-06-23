using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// KanColleViewer プラグインを表します。プラグインは、必ずこのコントラクト型をエクスポートしてください。
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// プラグインの初期化処理を実行します。
		/// </summary>
		void Initialize();
	}
}
