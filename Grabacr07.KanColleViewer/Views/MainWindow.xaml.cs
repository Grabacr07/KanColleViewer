using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// KanColleViewer のメイン ウィンドウを表します。
	/// </summary>
	partial class MainWindow
	{
		public static MainWindow Current { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			Current = this;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			// ToDo: 確認ダイアログを実装したかった…
			//e.Cancel = true;

			//var dialog = new ExitDialog { Owner = this, };
			//dialog.Show();

			base.OnClosing(e);
		}
	}
}
