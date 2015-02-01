using Grabacr07.KanColleViewer.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace CsvLogReader.ViewModels
{
	public class ShipBuildLogViewerViewModel : WindowViewModel
	{
		#region Lists 変更通知プロパティ

		private List<BuildStirngLists> _Lists;

		public List<BuildStirngLists> Lists
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

		public ShipBuildLogViewerViewModel()
		{
			if (File.Exists(MainFolder + "\\ShipBuildLog.csv"))
				this.Lists = new List<BuildStirngLists>(LogViewerViewModel.Current.LogDataList.ReturnBuildList(MainFolder + "\\ShipBuildLog.csv"));

			this.Title = "건조 기록 보관소";
		}
	}
}
