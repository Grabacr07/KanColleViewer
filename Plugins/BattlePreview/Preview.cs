using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.ViewModels;
using Grabacr07.KanColleViewer.Plugins.Views;
using System.ComponentModel.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "전투예보")]
	[ExportMetadata("Description", "계산이 끝난 전투결과를 표시해주는 플러그인입니다.")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@FreyYa")]

	public class Preview : IToolPlugin
	{
		private readonly BattlePreviewsViewModel battlePreviewsViewModel = new BattlePreviewsViewModel();
		public string ToolName
		{
			get { return "전투예보"; }
		}

		public object GetSettingsView()
		{
			return null;
		}
		public object GetToolView()
		{
			return new BattlePreviews { DataContext = this.battlePreviewsViewModel };
		}
	}
}
