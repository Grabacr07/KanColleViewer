using Grabacr07.KanColleViewer.Views;

namespace CsvLogReader.Catalogs
{
	/// <summary>
	/// ShipBuildLogViewer.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ShipBuildLogViewer
	{
		public ShipBuildLogViewer()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
