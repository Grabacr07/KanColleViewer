using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.ViewModels;
using Grabacr07.KanColleViewer.Plugins.Views;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[Export(typeof(ISettings))]
	[ExportMetadata("Guid", "45BF5FE6-7D81-4978-8B8A-84FD80BBEC10")]
	[ExportMetadata("Title", "MastarData")]
	[ExportMetadata("Description", "start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class MasterDataViewer : IPlugin, ITool, ISettings
	{
		private readonly PortalViewModel portalViewModel = new PortalViewModel();

		public void Initialize()
		{
		}

		string ITool.Name => "MasterView";

		object ITool.View => new Portal { DataContext = this.portalViewModel };

		object ISettings.View => new Settings();
	}
}
