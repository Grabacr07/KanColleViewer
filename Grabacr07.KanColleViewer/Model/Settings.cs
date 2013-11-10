using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model.Data.Xml;
using Livet;

namespace Grabacr07.KanColleViewer.Model
{
	public class Settings : NotificationObject
	{
		#region static members

		private static readonly string filePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net",
			"KanColleViewer",
			"Settings.xml");

		public static Settings Current { get; set; }

		public static void Load()
		{
			try
			{
				Current = filePath.ReadXml<Settings>();
			}
			catch (Exception ex)
			{
				Current = GetInitialSettings();
				Debug.WriteLine(ex);
			}
		}

		public static Settings GetInitialSettings()
		{
			return new Settings
			{
				ScreenshotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				ScreenshotFilename = "KanColle-{0:d04}.png",
				CanDisplayBuildingShipName = false,
			};
		}

		#endregion
		

		#region ScreenshotFolder 変更通知プロパティ

		private string _ScreenshotFolder;

		/// <summary>
		/// スクリーンショットの保存先フォルダーを取得または設定します。
		/// </summary>
		public string ScreenshotFolder
		{
			get { return this._ScreenshotFolder; }
			set
			{
				if (this._ScreenshotFolder != value)
				{
					this._ScreenshotFolder = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ScreenshotFilename 変更通知プロパティ

		private string _ScreenshotFilename;

		/// <summary>
		/// スクリーンショットのファイル名を取得または設定します。
		/// </summary>
		public string ScreenshotFilename
		{
			get { return this._ScreenshotFilename; }
			set
			{
				if (this._ScreenshotFilename != value)
				{
					this._ScreenshotFilename = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanDisplayBuildingShipName 変更通知プロパティ

		private bool _CanDisplayBuildingShipName;

		/// <summary>
		/// 建造中の艦の名前を表示するかどうかを示す値を取得または設定します。
		/// </summary>
		public bool CanDisplayBuildingShipName
		{
			get { return this._CanDisplayBuildingShipName; }
			set
			{
				if (this._CanDisplayBuildingShipName != value)
				{
					this._CanDisplayBuildingShipName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyBuildingComplete 変更通知プロパティ

		private bool _NotifyBuildingCompleted;

		public bool NotifyBuildingCompleted
		{
			get { return this._NotifyBuildingCompleted && Helper.IsWindows8OrGreater; }
			set
			{
				if (this._NotifyBuildingCompleted != value)
				{
					this._NotifyBuildingCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyExpeditionReturned 変更通知プロパティ

		private bool _NotifyExpeditionReturned;

		public bool NotifyExpeditionReturned
		{
			get { return this._NotifyExpeditionReturned && Helper.IsWindows8OrGreater; }
			set
			{
				if (this._NotifyExpeditionReturned != value)
				{
					this._NotifyExpeditionReturned = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyRepairingCompleted 変更通知プロパティ

		private bool _NotifyRepairingCompleted;

		public bool NotifyRepairingCompleted
		{
			get { return this._NotifyRepairingCompleted && Helper.IsWindows8OrGreater; }
			set
			{
				if (this._NotifyRepairingCompleted != value)
				{
					this._NotifyRepairingCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public void Save()
		{
			try
			{
				this.WriteXml(filePath);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
