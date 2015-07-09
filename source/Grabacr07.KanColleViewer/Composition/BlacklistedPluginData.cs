using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public class BlacklistedPluginData
	{
		public string FilePath { get; set; }

		public string Exception { get; set; }

		public PluginMetadata Metadata { get; set; }
	}
}
