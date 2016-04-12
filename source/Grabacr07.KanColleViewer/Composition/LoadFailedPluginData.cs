using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public class LoadFailedPluginData
	{
		public string FilePath { get; set; }

		public string Message { get; set; }

		public PluginMetadata Metadata { get; set; }
	}
}
