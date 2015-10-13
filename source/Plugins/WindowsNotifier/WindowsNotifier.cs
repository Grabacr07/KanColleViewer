using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IPlugin))]
	[ExportMetadata("Guid", "65E061E7-8A82-4CC6-835B-BC7E7DC233D2")]
	[ExportMetadata("Title", "WindowsNotifier")]
	[ExportMetadata("Description", "Windows OS의 기능 (토스트 알림,벌룬 알림)를 사용하여 알림을 보냅니다. 특정 음원파일을 알림음으로 송출합니다.")]
	[ExportMetadata("Version", "2.0.0.1")]
	[ExportMetadata("Author", "@Grabacr07, FreyYa")]
	public class WindowsNotifier : IPlugin
	{
		public void Initialize() { }
	}
}
