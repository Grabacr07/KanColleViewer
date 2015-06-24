using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public interface IPluginGuid
	{
		/// <summary>
		/// プラグインを表す GUID を取得します。
		/// </summary>
		string Guid { get; }
	}
}
