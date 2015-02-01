using Grabacr07.KanColleViewer.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace CsvLogReader.ViewModels
{
	public class DropLogViewerViewModel : WindowViewModel
	{		
		#region Lists 変更通知プロパティ

		private List<DropStringLists> _Lists;

		public List<DropStringLists> Lists
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
		public DropLogViewerViewModel()
		{
			if (File.Exists(MainFolder + "\\DropLog.csv")) 
				this.Lists = new List<DropStringLists>(LogViewerViewModel.Current.LogDataList.ReturnDropList(MainFolder + "\\DropLog.csv"));
				
			this.Title = "드롭 기록 보관소";
		}
	}
}
