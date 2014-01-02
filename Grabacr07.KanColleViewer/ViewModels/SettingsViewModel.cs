﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.IO;
using Settings = Grabacr07.KanColleViewer.Model.Settings;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel, INotifyDataErrorInfo
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

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder
		{
			get { return Directory.Exists(this.ScreenshotFolder); }
		}

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		public SupportedImageFormat ScreenshotImageFormat
		{
			get { return Settings.Current.ScreenshotImageFormat; }
			set
			{
				if (Settings.Current.ScreenshotImageFormat != value)
				{
					Settings.Current.ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
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
					KanColleClient.Current.Proxy.UpstreamProxyHost = value;
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
					KanColleClient.Current.Proxy.UpstreamProxyPort = numberPort;
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
					KanColleClient.Current.Proxy.UseProxyOnConnect = booleanValue;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region UseProxyForSSL 変更通知プロパティ

		public bool UseProxyForSSL
		{
			get { return Settings.Current.EnableSSLProxy; }
			set
			{
				if (Settings.Current.EnableSSLProxy != value)
				{
					Settings.Current.EnableSSLProxy = value;
					KanColleClient.Current.Proxy.UseProxyOnSSLConnect = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReSortieCondition 変更通知プロパティ

		private string _ReSortieCondition = Settings.Current.ReSortieCondition.ToString(CultureInfo.InvariantCulture);
		private string reSortieConditionError;

		public string ReSortieCondition
		{
			get { return this._ReSortieCondition; }
			set
			{
				if (this._ReSortieCondition != value)
				{
					ushort cond;
					if (ushort.TryParse(value, out cond) && cond <= 49)
					{
						Settings.Current.ReSortieCondition = cond;
						this.reSortieConditionError = null;
					}
					else
					{
						this.reSortieConditionError = "コンディション値は 0 ～ 49 の数値で入力してください。";
					}

					this._ReSortieCondition = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Libraries 変更通知プロパティ

		private IEnumerable<BindableTextViewModel> _Libraries;

		public IEnumerable<BindableTextViewModel> Libraries
		{
			get { return this._Libraries; }
			set
			{
				if (this._Libraries != value)
				{
					this._Libraries = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public bool HasErrors
		{
			get { return this.reSortieConditionError != null; }
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.Name = Properties.Resources.ViewModels_Settings;

			this.Libraries = App.ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

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


		public IEnumerable GetErrors(string propertyName)
		{
			var errors = new List<string>();

			switch (propertyName)
			{
				case "ReSortieCondition":
					if (this.reSortieConditionError != null)
					{
						errors.Add(this.reSortieConditionError);
					}
					break;
			}

			return errors.HasValue() ? errors : null;
		}

		protected void RaiseErrorsChanged([CallerMemberName]string propertyName = "")
		{
			if (this.ErrorsChanged != null)
			{
				this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}
	}
}
