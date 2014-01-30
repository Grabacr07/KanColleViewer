using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	/// <summary>
	/// 装備一覧ウィンドウを表します。
	/// </summary>
	partial class SlotItemCatalogWindow
	{
		public SlotItemCatalogWindow()
		{
			this.InitializeComponent();

			MainWindow.Current.Closed += (sender, args) => this.Close();
		}
	}
}
