using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace Grabacr07.KanColleViewer.Plugins
{
	/// <summary>
	/// Windows 8 のトースト通知機能を提供します。
	/// </summary>
	public class Toast
	{
		#region static members

		/// <summary>
		/// トースト通期機能をサポートしているかどうかを示す値を取得します。
		/// </summary>
		/// <returns>
		/// 動作しているオペレーティング システムが Windows 8 (NT 6.2) 以降の場合は true、それ以外の場合は false。
		/// </returns>
		public static bool IsSupported
		{
			get
			{
				var version = Environment.OSVersion.Version;
				return (version.Major == 6 && version.Minor >= 2) || version.Major > 6;

			}
		}

		#endregion

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
			remove { this.toast.Dismissed -= value; }
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
	}
}
