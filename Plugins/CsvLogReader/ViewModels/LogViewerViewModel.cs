using Livet;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

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

		public LogViewerViewModel()
		{

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
	}
}
