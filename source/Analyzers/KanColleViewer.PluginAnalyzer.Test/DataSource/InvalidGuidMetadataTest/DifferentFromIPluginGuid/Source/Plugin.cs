using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

namespace DifferentFromIPluginGuid
{
	[Export(typeof(IPlugin))]
	[ExportMetadata("Guid", "45BF5FE6-7D81-4978-8B8A-84FD80BBEC10")]
	[ExportMetadata("Title", "MastarData")]
	[ExportMetadata("Description", "start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	class Hoge : IPlugin
	{
		public void Initialize() { }
	}

	[Export(typeof(IPlugin))]
	class Piyo : IPlugin
	{
		public void Initialize() { }
	}
}
