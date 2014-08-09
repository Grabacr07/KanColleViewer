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
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "MastarData")]
	[ExportMetadata("Description", "start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class MasterDataViewer : IToolPlugin
	{
		private readonly PortalViewModel portalViewModel = new PortalViewModel();

		public string ToolName
		{
			get { return "MasterView"; }
		}


		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			return new Portal { DataContext = this.portalViewModel };
		}
	}
}
