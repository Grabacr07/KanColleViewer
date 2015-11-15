using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.Messaging.IO;
using Logger.Models;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Serialization;

namespace Logger.ViewModels
{
	public class LoggerViewModel : ViewModel
	{
		private LoggerSettings Settings => KanColleLogger.Settings;

		private static bool IsWindows8OrGreater => KanColleLogger.IsWindows8OrGreater;

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

		public string Location
		{
			get { return Settings.Location.Value; }
			set
			{
				if (value != Settings.Location.Value)
				{
					Settings.Location.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public string TimestampFormat
		{
			get { return Settings.DateTimeFormat.Value; }
			set
			{
				if (value != Settings.DateTimeFormat.Value)
				{
					Settings.DateTimeFormat.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public string DefaultTimestampFormat => LoggerSettings.OldStyleTimestampFormat;

		public bool UseJapaneseTime
		{
			get { return Settings.DateTimeUseJapanese.Value; }
			set
			{
				if (value != Settings.DateTimeUseJapanese.Value)
				{
					Settings.DateTimeUseJapanese.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public ObservableCollection<LoggerBase> Loggers => KanColleLogger.Loggers;

		public LoggerViewModel()
		{
			GeneralSettings.Culture.Subscribe(x => ResourceService.Current.ChangeCulture(x)).AddTo(this);

			Settings.Location
				.Subscribe(x => this.CanOpenDestination = Directory.Exists(x))
				.AddTo(this);
		}

		public void OpenDestinationSelectionDialog()
		{
			var message = new FolderSelectionMessage("FolderDialog.Logger.Open")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenDestination ? Settings.Location : ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.Location = message.Response;
			}
		}

		public void OpenLogsFolder()
		{
			if (!this.CanOpenDestination) return;

			try
			{
				Process.Start(this.Location);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

	}
}
