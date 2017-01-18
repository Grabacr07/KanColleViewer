using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// Web 브라우저를 표시하기 위한 서브 윈도우를 정의합니다.
	/// </summary>
	partial class BrowserWindow
	{
		/// <summary>
		/// 이 윈도우에 호스트되어있는 <see cref="System.Windows.Controls.WebBrowser"/> 객체를 반환합니다.
		/// </summary>
		public WebBrowser WebBrowser => this.webBrowser;

		public BrowserWindow()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
