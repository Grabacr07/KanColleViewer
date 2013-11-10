using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Win32;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// Windows 8 のトースト通知機能を提供します。
	/// </summary>
	public class Toast
	{
		public const string AppId = "Grabacr07.KanColleViewer";

		private readonly ToastNotification toast;

		public event TypedEventHandler<ToastNotification, object> Activated
		{
			add { this.toast.Activated += value; }
			remove { this.toast.Activated -= value; }
		}

		public event TypedEventHandler<ToastNotification, ToastDismissedEventArgs> Dismissed
		{
			add { this.toast.Dismissed += value; }
			remove { this.toast.Dismissed += value; }
		}

		public event TypedEventHandler<ToastNotification, ToastFailedEventArgs> ToastFailed
		{
			add { this.toast.Failed += value; }
			remove { this.toast.Failed -= value; }
		}

		public Toast(string header, string body)
		{
			var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

			var stringElements = toastXml.GetElementsByTagName("text");
			if (stringElements.Length == 2)
			{
				stringElements[0].AppendChild(toastXml.CreateTextNode(header));
				stringElements[1].AppendChild(toastXml.CreateTextNode(body));
			}

			this.toast = new ToastNotification(toastXml);
		}

		public void Show()
		{
			ToastNotificationManager.CreateToastNotifier(AppId).Show(this.toast);
		}


		#region static members

		/// <summary>
		/// トースト通期機能をサポートしているかどうかを示す値を取得します。
		/// </summary>
		/// <returns>
		/// 動作しているオペレーティング システムが Windows 8 (NT 6.2) 以降の場合は true、それ以外の場合は false。
		/// </returns>
		public static bool IsSupported
		{
			get { return Helper.IsWindows8OrGreater; }
		}

		public static void Show(
			string header, string body,
			Action activated, Action<ToastDismissalReason> dismissed = null, Action<Exception> failed = null)
		{
			var toast = new Toast(header, body);
			toast.Activated += (sender, args) => activated();
			if (dismissed != null) toast.Dismissed += (sender, args) => dismissed(args.Reason);
			if (failed != null) toast.ToastFailed += (sender, args) => failed(args.ErrorCode);

			toast.Show();
		}

		public static bool TryInstallShortcut()
		{
			var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs\\提督業も忙しい！.lnk";

			if (!File.Exists(shortcutPath))
			{
				InstallShortcut(shortcutPath);
				return true;
			}
			return false;
		}

		private static void InstallShortcut(string shortcutPath)
		{
			var exePath = Process.GetCurrentProcess().MainModule.FileName;
			var newShortcut = (IShellLinkW)new CShellLink();

			ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
			ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

			var newShortcutProperties = (IPropertyStore)newShortcut;

			using (var appId = new PropVariant(AppId))
			{
				ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
				ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
			}

			var newShortcutSave = (IPersistFile)newShortcut;

			ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
		}

		#endregion
	}
}
