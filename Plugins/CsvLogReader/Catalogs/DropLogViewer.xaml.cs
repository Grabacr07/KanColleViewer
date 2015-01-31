using Grabacr07.KanColleViewer.Views;

namespace CsvLogReader.Catalogs
{
	/// <summary>
	/// DropLogViewer.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class DropLogViewer
	{
		public DropLogViewer()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
