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
using MetroRadiance.UI.Controls;

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	/// <summary>
	/// Interaction logic for PresetFleetAddWindow.xaml
	/// </summary>
	public partial class PresetFleetAddWindow
	{
		public PresetFleetAddWindow()
		{
			this.InitializeComponent();

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
