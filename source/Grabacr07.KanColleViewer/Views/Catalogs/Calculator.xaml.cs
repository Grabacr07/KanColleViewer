
namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class Calculator
	{
		public Calculator()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
