using System.Windows;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// 대파가 발생한경우 이 창으로 진격버튼을 가립니다. 최종구현은 모항에 돌아갔을때도 창이 자동으로 꺼지는것을 목표
	/// 현재 가로모드하고 세로모드가 사양이 다르기때문에 좌표값을 일일히 따로 지정해줘야하는 불편함이 있음
	/// 
	/// </summary>
	public partial class CriticalDialog
	{
		public CriticalDialog()
		{
			InitializeComponent();
			
			MainWindow.Current.Closed += (sender, args) => this.Close();
		}

		private void CloseBox(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
