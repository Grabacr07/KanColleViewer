using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public static class GeneralSettings
	{
		/// <summary>
		/// 지역을 설정합니다.
		/// </summary>
		public static SerializableProperty<string> Culture { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer) { AutoSave = true };

		/// <summary>
		/// 어플리케이션이 프록시 모드(브라우저를 표시하지 않고 프록시로만 사용하는 모드)로 작동할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> IsProxyMode { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 어플리케이션을 종료하려고 할 때 경고를 표시하는 조건을 설정합니다.
		/// </summary>
		public static SerializableProperty<ExitConfirmationType> ExitConfirmationType { get; }
			= new SerializableProperty<ExitConfirmationType>(GetKey(), Providers.Viewer, Models.ExitConfirmationType.None) { AutoSave = true };

		/// <summary>
		/// 새로고침 버튼을 누르려고 할 때 경고를 표시하는 조건을 설정합니다.
		/// </summary>
		public static SerializableProperty<ExitConfirmationType> RefreshConfirmationType { get; }
			= new SerializableProperty<ExitConfirmationType>(GetKey(), Providers.Viewer, Models.ExitConfirmationType.InSortieOnly) { AutoSave = true };

		/// <summary>
		/// MMCSS 사용 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> MMCSSEnabled { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true) { AutoSave = true };

		/// <summary>
		/// 브라우저 확대 배율을 설정합니다.
		/// </summary>
		public static SerializableProperty<double> BrowserZoomFactor { get; }
			= new SerializableProperty<double>(GetKey(), Providers.Local, 1.0);

		/// <summary>
		/// 유저 스타일 시트 (CSS)를 설정합니다.
		/// </summary>
		public static SerializableProperty<string> UserStyleSheet { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, Properties.Settings.Default.OverrideStyleSheet) { AutoSave = true };

		/// <summary>
		/// 작업표시줄 인디케이터에서 사용할 플러그인 기능을 구별하는 ID를 설정합니다.
		/// </summary>
		public static SerializableProperty<string> TaskbarProgressSource { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, "") { AutoSave = true };

		/// <summary>
		/// 출격중 작업표시줄 인디케이터에서 사용할 플러그인 기능을 구별하는 ID를 설정합니다.
		/// </summary>
		public static SerializableProperty<string> TaskbarProgressSourceWhenSortie { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, "") { AutoSave = true };

		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(GeneralSettings) + "." + propertyName;
		}
	}
}
