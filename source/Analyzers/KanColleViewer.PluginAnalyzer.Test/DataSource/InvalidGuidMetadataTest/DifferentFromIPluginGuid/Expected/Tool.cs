using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

namespace DifferentFromIPluginGuid
{
	[Export(typeof(ITool))]
	[ExportMetadata("Guid", "45BF5FE6-7D81-4978-8B8A-84FD80BBEC10")]
	class Tool : ITool
	{
		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public object View
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
