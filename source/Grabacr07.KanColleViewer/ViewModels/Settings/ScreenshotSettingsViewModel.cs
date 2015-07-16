using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.Messaging.IO;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class ScreenshotSettingsViewModel : ViewModel
	{
		#region CanOpenDestination 変更通知プロパティ

		private bool _CanOpenDestination;

		public bool CanOpenDestination
		{
			get { return this._CanOpenDestination; }
			set
			{
				if (this._CanOpenDestination != value)
				{
					this._CanOpenDestination = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ScreenshotSettingsViewModel()
		{
			ScreenshotSettings.Destination
				.Subscribe(x => this.CanOpenDestination = Directory.Exists(x))
				.AddTo(this);
		}

		public void OpenDestinationSelectionDialog()
		{
			var message = new FolderSelectionMessage("FolderDialog.Screenshot.Open")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenDestination ? ScreenshotSettings.Destination : ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				ScreenshotSettings.Destination.Value = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (!this.CanOpenDestination) return;

			try
			{
				Process.Start(ScreenshotSettings.Destination);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
