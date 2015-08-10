using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IPlugin))]
	[ExportMetadata("Guid", "6EDE38C8-412D-4A73-8FE3-A9D20EB9F0D2")]
	[ExportMetadata("Title", "WindowsNotifier")]
	[ExportMetadata("Description", "Windows OS の機能 (トースト通知・バルーン通知) を使用して通知します。")]
	[ExportMetadata("Version", "2.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class WindowsNotifier : IPlugin
	{
		public void Initialize() { }
	}
}
