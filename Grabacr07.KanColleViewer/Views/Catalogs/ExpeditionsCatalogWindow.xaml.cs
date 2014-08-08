using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class ExpeditionsCatalogWindow
	{
		public string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		public ExpeditionsCatalogWindow()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
