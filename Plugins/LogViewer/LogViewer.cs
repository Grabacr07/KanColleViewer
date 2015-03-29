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
	[ExportMetadata("Title", "LogViewer")]
    [ExportMetadata("Description", "ドロップ・建造・開発ログを表示する。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "+PaddyXu")]
	public class LogViewer : IToolPlugin
	{
		private readonly PortalViewModel logViewerViewModel = new PortalViewModel();

		public string ToolName
		{
            get { return "LogViewer"; }
		}


		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			return new Portal { DataContext = this.logViewerViewModel };
		}
	}
}
