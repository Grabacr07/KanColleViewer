using Grabacr07.KanColleViewer.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace CsvLogReader.ViewModels
{
	public class ItemBuildLogViewerViewModel : WindowViewModel
	{		
		#region Lists 変更通知プロパティ

		private List<ItemStringLists> _Lists;

		public List<ItemStringLists> Lists
		{
			get { return this._Lists; }
			set
			{
				if (this._Lists != value)
				{
					this._Lists = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		private string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		public ItemBuildLogViewerViewModel()
		{
			if (File.Exists(MainFolder + "\\ItemBuildLog.csv"))
				this.Lists = new List<ItemStringLists>(LogViewerViewModel.Current.LogDataList.ReturnItemList(MainFolder + "\\ItemBuildLog.csv"));

			this.Title = "장비개발 기록 보관소";
		}
	}
}
