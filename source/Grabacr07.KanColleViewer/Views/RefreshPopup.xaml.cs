
namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// RefreshPopup.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class RefreshPopup
	{
		public RefreshPopup()
		{
			InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
