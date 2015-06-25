using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class ToastNotifier : NotifierBase
	{
		public override bool IsSupported
		{
			get { return Toast.IsSupported; }
		}

		protected override void InitializeCore()
		{
			try
			{
				var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs\\提督業も忙しい！.lnk";
#if DEBUG
				if (!File.Exists(shortcutPath))
				{
					InstallShortcut(shortcutPath);
				}
#else
				InstallShortcut(shortcutPath);
#endif
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
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

		protected override void ShowCore(string header, string body, Action activated, Action<Exception> failed)
		{
			var toast = new Toast(header, body);
			toast.Activated += (sender, args) => activated();
			if (failed != null) toast.ToastFailed += (sender, args) => failed(args.ErrorCode);

			toast.Show();
		}
	}
}
