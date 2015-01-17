
namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// BattlePreview.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class BattlePreviewsPopUp
	{
		public BattlePreviewsPopUp()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
