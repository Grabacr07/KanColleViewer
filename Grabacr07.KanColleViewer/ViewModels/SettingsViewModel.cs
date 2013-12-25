using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.IO;
using Settings = Grabacr07.KanColleViewer.Model.Settings;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel
	{
		#region ScreenshotFolder 変更通知プロパティ

		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set
			{
				if (Settings.Current.ScreenshotFolder != value)
				{
					Settings.Current.ScreenshotFolder = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("CanOpenScreenshotFolder");
				}
			}
		}

		#endregion

		#region ProxyHost 変更通知プロパティ

		public string ProxyHost
		{
			get { return Settings.Current.ProxyHost; }
			set
			{
				if (Settings.Current.ProxyHost != value)
				{
					Settings.Current.ProxyHost = value;
					KanColleWrapper.KanColleClient.Current.Proxy.UpstreamProxyHost = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ProxyPort 変更通知プロパティ

		public string ProxyPort
		{
			get { return Settings.Current.ProxyPort.ToString(); }
			set
			{
				UInt16 numberPort;
				if (UInt16.TryParse(value, out numberPort))
				{
					Settings.Current.ProxyPort = numberPort;
					KanColleWrapper.KanColleClient.Current.Proxy.UpstreamProxyPort = numberPort;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region UseProxy 変更通知プロパティ
		public string UseProxy
		{
			get { return Settings.Current.EnableProxy.ToString(); }
			set
			{
				bool booleanValue;
				if (Boolean.TryParse(value, out booleanValue))
				{
					Settings.Current.EnableProxy = booleanValue;
					KanColleWrapper.KanColleClient.Current.Proxy.UseProxyOnConnect = booleanValue;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region UseProxyForSSL 変更通知プロパティ
		public string UseProxyForSSL
		{
			get { return Settings.Current.EnableSSLProxy.ToString(); }
			set
			{
				bool booleanValue;
				if (Boolean.TryParse(value, out booleanValue))
				{
					Settings.Current.EnableSSLProxy = booleanValue;
					KanColleWrapper.KanColleClient.Current.Proxy.UseProxyOnSSLConnect = booleanValue;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder
		{
			get { return Directory.Exists(this.ScreenshotFolder); }
		}
		#endregion


		public SettingsViewModel()
		{
			this.Name = "設定";

			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
		}

		public void OpenScreenshotFolderSelectionDialog()
		{
			var message = new FolderSelectionMessage("OpenFolderDialog/Screensgot")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenScreenshotFolder
					? this.ScreenshotFolder
					: ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.ScreenshotFolder = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (this.CanOpenScreenshotFolder)
			{
				try
				{
					Process.Start(this.ScreenshotFolder);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void ClearZoomFactor()
		{
			App.ViewModelRoot.Messenger.Raise(new ZoomMessage { MessageKey = "WebBrowser/Zoom", ZoomFactor = 100 });
		}

		public void SetLocationLeft()
		{
			App.ViewModelRoot.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window/Location", Left = 0.0 });
		}
	}
}
