using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class CalExp
	{
		public CalExp()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
