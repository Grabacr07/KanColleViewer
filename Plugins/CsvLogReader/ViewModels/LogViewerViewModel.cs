using Livet;
using Livet.Messaging;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace LogReader.ViewModels
{
	public class LogViewerViewModel : ViewModel
	{
		//#region static members
		//private static readonly LogViewerViewModel current = new LogViewerViewModel();

		//public static LogViewerViewModel Current
		//{
		//	get { return current; }
		//}
		//#endregion
		//public LogDataList LogDataList { get; set; }

		public LogViewerViewModel()
		{
			//LogDataList = new LogDataList();
		}
		public void ShowLogViewer()
		{
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			foreach (Process process in Process.GetProcesses())
			{
				if (process.MainWindowTitle == "제독업무도 바빠! 기록열람" && process.ProcessName.StartsWith("LogArchive"))
				{
					process.Kill();
				}
			}
			var Applocate = Path.Combine(MainFolder, "LogArchive.exe");
			Process.Start(Applocate);
		}
		//public void ShowDropLogViewer()
		//{
		//	var catalog = new DropLogViewerViewModel();
		//	var message = new TransitionMessage(catalog, "Show/DropLogViewer");
		//	this.Messenger.RaiseAsync(message);
		//}
		//public void ShowShipBuildLogViewer()
		//{
		//	var catalog = new ShipBuildLogViewerViewModel();
		//	var message = new TransitionMessage(catalog, "Show/ShipBuildLogViewer");
		//	this.Messenger.RaiseAsync(message);
		//}
		//public void ShowItemBuildLogViewer()
		//{
		//	var catalog = new ItemBuildLogViewerViewModel();
		//	var message = new TransitionMessage(catalog, "Show/ItemBuildLogViewer");
		//	this.Messenger.RaiseAsync(message);
		//}
	}
}
