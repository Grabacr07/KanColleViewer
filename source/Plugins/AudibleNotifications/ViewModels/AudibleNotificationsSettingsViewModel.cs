using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Plugins.Models;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.Messaging.IO;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
	public class AudibleNotificationsSettingsViewModel : ViewModel
	{
		#region CanOpenDestination 変更通知プロパティ

		private bool _CanOpenDestination;

		private static bool IsWindows8OrGreater => AudibleNotifications.IsWindows8OrGreater;

		private static AudibleNotifierSettings Settings => AudibleNotifications.Settings;

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

		#region TypeSettings 変更通知プロパティ

		public ObservableCollection<NotificationType> TypeSettings { get; set; }

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

		public int Volume
		{
			get { return Settings.Volume.Value; }
			set
			{
				if (value != Settings.Volume.Value)
				{
					Settings.Volume.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}

		// public ObservableCollection<> 

		public AudibleNotificationsSettingsViewModel()
		{
			TypeSettings = new ObservableCollection<NotificationType>();

			foreach (var setting in Settings.TypeSettings)
			{
				TypeSettings.Add(new NotificationType {Type = setting.Key, Enabled = setting.Value});
			}

			GeneralSettings.Culture.Subscribe(x => AudibleNotificationsResourceService.Current.ChangeCulture(x)).AddTo(this);

			Settings.Location
				.Subscribe(x => this.CanOpenDestination = Directory.Exists(x))
				.AddTo(this);
		}

		public void OpenDestinationSelectionDialog()
		{
			var message = new FolderSelectionMessage("FolderDialog.AudibleNotifications.Open")
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

		public void OpenAudibleNotificationsFolder()
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

		public void CreateSubfolders()
		{
			if (!this.CanOpenDestination) return;

			foreach (var type in TypeSettings.Where(type => type.Enabled))
			{
				try
				{
					Directory.CreateDirectory(Path.Combine(this.Location, type.Type));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("AudibleNotifications: Failed to create folder {0} for type {1}; reason: {2}.", Path.Combine(this.Location, type.Type), type.Type, ex.Message);
				}
			}
		}

		public class NotificationType : NotificationObject
		{
			private static AudibleNotifierSettings Settings => AudibleNotifications.Settings;

			public string Type { get; set; }

			private bool enabled;

			public bool Enabled
			{
				get
				{
					return this.enabled;
				}
				set
				{
					if (value != this.enabled)
					{
						this.enabled = value;
						Settings.TypeSettings[this.Type].Value = enabled;

						this.RaisePropertyChanged();
					}
				}
			}
		}
	}
}