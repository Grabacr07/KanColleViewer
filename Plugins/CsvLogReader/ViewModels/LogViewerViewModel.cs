using Livet;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace LogReader.ViewModels
{
	/// <summary>
	/// http://xarfox.tistory.com/51
	/// </summary>
	public class LogViewerViewModel : ViewModel
	{
		bool isExecuting = false;
		// FindWindow 사용을 위한 코드
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string strClassName, string StrWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern void SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private const int SW_SHOWNORMAL = 1;
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
			Process[] process = Process.GetProcesses();
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			foreach (Process proc in process)
			{

				if (proc.ProcessName.Equals("LogArchive"))
				//  Pgm_FileName 프로그램의 실행 파일[.exe]를 제외한 파일명
				{
					isExecuting = true;
					break;
				}

				else
					isExecuting = false;
			}
			if (isExecuting)
			{IntPtr procHandler = FindWindow(null, "제독업무도 바빠! 기록열람");
				ShowWindow(procHandler, SW_SHOWNORMAL);
				SetForegroundWindow(procHandler);
			}
			else if(!isExecuting)
			{
				if (File.Exists(Path.Combine(MainFolder, "LogArchive.exe")))
				{
					Process MyProcess = new Process();
					MyProcess.StartInfo.FileName = "LogArchive.exe";
					MyProcess.StartInfo.WorkingDirectory = MainFolder;
					MyProcess.Start();
					MyProcess.Refresh();
				}
			}

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
