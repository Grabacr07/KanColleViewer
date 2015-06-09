using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	[Serializable]
	public class LoadFailurePluginData
	{
		public string Filename { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public string Author { get; set; }

		public string Description { get; set; }

		public Exception Exception { get; set; }
	}
}
