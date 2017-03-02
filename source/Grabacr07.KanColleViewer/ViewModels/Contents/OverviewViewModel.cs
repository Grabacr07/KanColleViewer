using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleViewer.Views.Catalogs;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class OverviewViewModel : TabItemViewModel
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

		public override string Name
		{
			get { return Resources.IntegratedView; }
			protected set { throw new NotImplementedException(); }
		}

		public InformationViewModel Content { get; }

		#region Vertical Visibility
		private Visibility _Vertical;
		public Visibility Vertical
		{
			get { return this._Vertical; }
			set
			{
				if (value == this._Vertical) return;
				this._Vertical = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Horizontal VIsibility
		private Visibility _Horizontal;
		public Visibility Horizontal
		{
			get { return this._Horizontal; }
			set
			{
				if (value == this._Horizontal) return;
				this._Horizontal = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public KanColleWindowSettings Settings { get; }

		public OverviewViewModel(InformationViewModel owner)
		{
			this.Content = owner;

			this.Settings = SettingsHost.Instance<KanColleWindowSettings>();

			if (this.Settings?.Dock == Dock.Right 
				|| this.Settings?.Dock == Dock.Left 
				|| this.Settings?.IsSplit)
			{
				this.Vertical = Visibility.Collapsed;
				this.Horizontal = Visibility.Visible;
			}
			else
			{
				this.Vertical = Visibility.Visible;
				this.Horizontal = Visibility.Collapsed;
			}
		}


		public void Jump(string tabName)
		{
			TabItemViewModel target = null;

			switch (tabName)
			{
				case "Fleets":
					target = this.Content.Fleets;
					break;
				case "Expeditions":
					target = this.Content.Expeditions;
					break;
				case "Quests":
					target = this.Content.Quests;
					break;
				case "Repairyard":
					target = this.Content.Shipyard;
					break;
				case "Dockyard":
					target = this.Content.Shipyard;
					break;
			}

			if (target != null) target.IsSelected = true;
		}

		public void ShowShipCatalog()
		{
			var catalog = new ShipCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(ShipCatalogWindow));
		}

		public void ShowSlotItemCatalog()
		{
			var catalog = new SlotItemCatalogViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(SlotItemCatalogWindow));
		}
		public void Calculator()
		{
			var catalog = new CalculatorViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(Calculator));
		}
		public void ExpeditionsCatalogWindow()
		{
			var catalog = new ExpeditionsCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(ExpeditionsCatalogWindow));
		}
		public void ShowNotePad()
		{
			var catalog = new NotePadViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(NotePad));
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
			{
				IntPtr procHandler = FindWindow(null, "제독업무도 바빠! 기록열람");
				ShowWindow(procHandler, SW_SHOWNORMAL);
				SetForegroundWindow(procHandler);
			}
			else if (!isExecuting)
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
        public void ShowNdockShipCatalog()
		{
			var catalog = new NeedNdockShipCatalogWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(NeedNdockShipCatalogWindow));
		}
		public void ShowRemodelWindow()
		{
			var catalog = new RemodelListWindowViewModel();
			WindowService.Current.MainWindow.Transition(catalog, typeof(RemodelListWindow));
		}
	}
}
