
namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// RefreshPopup.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class RefreshPopup
	{
		public static RefreshPopup Current { get; private set; }
		public RefreshPopup()
		{
			InitializeComponent();
			Current = this;

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
