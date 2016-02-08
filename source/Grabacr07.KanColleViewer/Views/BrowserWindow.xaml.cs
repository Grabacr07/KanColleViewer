using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// Web ブラウザーを表示するためのサブ ウィンドウを表します。
	/// </summary>
	partial class BrowserWindow
	{
		/// <summary>
		/// このウィンドウにホストされている <see cref="System.Windows.Controls.WebBrowser"/> オブジェクトを取得します。
		/// </summary>
		public WebBrowser WebBrowser => this.webBrowser;

		public BrowserWindow()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
