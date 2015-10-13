using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	[Serializable]
	public class PluginMetadata
	{
		public string Title { get; set; }

		public string Description { get; set; }

		public string Version { get; set; }

		public string Author { get; set; }
	}
}
