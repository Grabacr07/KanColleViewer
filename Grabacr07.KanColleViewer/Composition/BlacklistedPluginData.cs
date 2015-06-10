using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	[Serializable]
	public class BlacklistedPluginData
	{
		public string FilePath { get; set; }

		public string Exception { get; set; }
	}
}
