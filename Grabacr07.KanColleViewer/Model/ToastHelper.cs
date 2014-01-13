using Grabacr07.KanColleViewer.Win32;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Grabacr07.KanColleViewer.Model
{
	public class ToastHelper : INotifyWrapper
	{
		public void Initialize()
		{
			TryInstallShortcut();
		}

		private static bool TryInstallShortcut()
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

			using (var appId = new PropVariant(Toast.AppId))
			{
				ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
				ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
			}

			var newShortcutSave = (IPersistFile)newShortcut;

			ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
		}

		public void Show(
			string header, string body, 
			Action activated, Action<ToastDismissalReason> dismissed = null, Action<Exception> failed = null)
		{
			var toast = new Toast(header, body);
			toast.Activated += (sender, args) => activated();
			if (dismissed != null) toast.Dismissed += (sender, args) => dismissed(args.Reason);
			if (failed != null) toast.ToastFailed += (sender, args) => failed(args.ErrorCode);

			toast.Show();
		}
	}
}
