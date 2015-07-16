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
using System.Windows.Shapes;

namespace Grabacr07.KanColleViewer.Views
{
	/// <summary>
	/// ContentWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class InformationWindow
	{
		public InformationWindow()
		{
			InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
