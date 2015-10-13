using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Codeplex.Data;

namespace Grabacr07.Tools.JsonViewer
{
	partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Parse(object sender, RoutedEventArgs e)
		{
			var target = this.TextBox.Text;
			if (!string.IsNullOrEmpty(target))
			{
				target = target.Replace("&quot;", "\"");
				var obj = DynamicJson.Parse(target);
			}
			// ↖ ここにブレークポイントを仕掛けよう
		}
	}
}
