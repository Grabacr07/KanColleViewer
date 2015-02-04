
namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	/// <summary>
	/// NeedNdockShipCatalogWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	partial class NeedNdockShipCatalogWindow
	{
		public NeedNdockShipCatalogWindow()
		{
			InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
