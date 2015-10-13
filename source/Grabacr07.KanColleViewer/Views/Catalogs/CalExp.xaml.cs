
namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class CalExp
	{
		public CalExp()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
