using CsvLogReader.ViewModels;
using Grabacr07.KanColleViewer.Composition;
using System.ComponentModel.Composition;

namespace CsvLogReader
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "KanColleLogReader")]
	[ExportMetadata("Description", "csv파일로 된 로그를 읽어들입니다")]
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
