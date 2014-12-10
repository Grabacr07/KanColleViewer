using System;
using Windows.Data.Xml.Dom;
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
		CustomSound sound = new CustomSound();

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
			XmlDocument doc = new XmlDocument(); ;
			doc = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

			var test = doc.DocumentElement;
			XmlElement audio = doc.CreateElement("audio");
			if (!string.IsNullOrEmpty(sound.FileCheck(header))) audio.SetAttribute("silent", "true");
			test.AppendChild(audio);
			doc = test.OwnerDocument;

			var toastXml = doc;

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
