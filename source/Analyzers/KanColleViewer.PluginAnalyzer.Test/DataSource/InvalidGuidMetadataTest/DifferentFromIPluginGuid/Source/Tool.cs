using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

namespace DifferentFromIPluginGuid
{
	[Export(typeof(ITool))]
	[ExportMetadata("Guid", "invalid")]
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
