using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;

namespace Grabacr07.KanColleViewer.Models.Migration
{
	/// <summary>互換性のために残されています。</summary>
	[Obsolete]
	[Serializable]
	[XmlRoot("Settings")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	// ReSharper disable once InconsistentNaming
	public class _Settings : NotificationObject
	{
		#region static members

		private static readonly string filePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net",
			"KanColleViewer",
			"Settings.xml");

		public static _Settings Current { get; set; }

		public static void Load()
		{
			if (!File.Exists(filePath)) return;

			try
			{
				Current = filePath.ReadXml<_Settings>();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);

				return;
			}

			// 設定のマイグレーション

			GeneralSettings.Culture.Value = Current.Culture;
			GeneralSettings.BrowserZoomFactor.Value = Current.BrowserZoomFactorPercentage / 100.0;

			ScreenshotSettings.Destination.Value = Current.ScreenshotFolder;
			ScreenshotSettings.Filename.Value = Current.ScreenshotFilename;
			ScreenshotSettings.Format.Value = Current.ScreenshotImageFormat;

			KanColleSettings.CanDisplayBuildingShipName.Value = Current.CanDisplayBuildingShipName;
			KanColleSettings.NotifyBuildingCompleted.Value = Current.NotifyBuildingCompleted;
			KanColleSettings.NotifyExpeditionReturned.Value = Current.NotifyExpeditionReturned;
			KanColleSettings.NotifyRepairingCompleted.Value = Current.NotifyRepairingCompleted;
			KanColleSettings.NotifyFleetRejuvenated.Value = Current.NotifyFleetRejuvenated;

			if (Current.KanColleClientSettings != null)
			{
				KanColleSettings.ViewRangeCalcType.Value = Current.KanColleClientSettings.ViewRangeCalcType ?? KanColleSettings.ViewRangeCalcType.Default;
				KanColleSettings.NotificationShorteningTime.Value = Current.KanColleClientSettings.NotificationShorteningTime;
				KanColleSettings.ReSortieCondition.Value = Current.KanColleClientSettings.ReSortieCondition;
			}

			if (Current.ProxySettings != null)
			{
				NetworkSettings.Proxy.Type.Value = Current.ProxySettings.Type;
				NetworkSettings.Proxy.Host.Value = Current.ProxySettings.Host;
				NetworkSettings.Proxy.Port.Value = Current.ProxySettings.Port;
			}

			try
			{
				File.Delete(filePath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		#endregion


		public bool CanCloseWithoutConfirmation { get; set; }

		public string ScreenshotFolder { get; set; }

		public string ScreenshotFilename { get; set; }

		public SupportedImageFormat ScreenshotImageFormat { get; set; }

		public bool CanDisplayBuildingShipName { get; set; }

		public bool NotifyBuildingCompleted { get; set; }

		public bool NotifyExpeditionReturned { get; set; }

		public bool NotifyRepairingCompleted { get; set; }

		public bool NotifyFleetRejuvenated { get; set; }

		public _ProxySettings ProxySettings { get; set; }

		public bool TopMost { get; set; }

		public string Culture { get; set; }

		public int BrowserZoomFactorPercentage { get; set; }

		public bool IsProxyMode { get; set; }

#pragma warning disable 612
		public _KanColleClientSettings KanColleClientSettings { get; set; }
#pragma warning restore 612

	}
}
