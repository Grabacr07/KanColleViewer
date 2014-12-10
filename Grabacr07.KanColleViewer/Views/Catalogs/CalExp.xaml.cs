
namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class CalExp
	{
		public CalExp()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
