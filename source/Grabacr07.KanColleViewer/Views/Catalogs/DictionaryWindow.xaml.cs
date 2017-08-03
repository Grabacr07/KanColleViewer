namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class DictionaryWindow
	{
		public DictionaryWindow()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
