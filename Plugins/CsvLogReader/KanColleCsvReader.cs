using LogReader.ViewModels;
using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;

namespace LogReader
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "KanColleLogReader")]
	[ExportMetadata("Description", "로그를 읽는 프로그램을 실행시킵니다")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@FreyYa312")]
	public class KanColleCsvReader : IToolPlugin
	{
		private readonly LogViewerViewModel viewmodel = new LogViewerViewModel();
		public string ToolName
		{
			get { return "Logs"; }
		}

		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			return new LogViewer { DataContext = this.viewmodel, };
		}
	}
}
