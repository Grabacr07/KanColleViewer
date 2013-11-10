using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Interop;
using Grabacr07.Desktop.Metro.Win32;

namespace Grabacr07.Desktop.Metro
{
	/// <summary>
	/// Windows 8.1 の Per-Monitor DPI 機能へアクセスします。
	/// </summary>
	public static class PerMonitorDpiService
	{
		/// <summary>
		/// Per-Monitor DPI 機能をサポートしているかどうかを示す値を取得します。
		/// </summary>
		/// <returns>
		/// 動作しているオペレーティング システムが Windows 8.1 (NT 6.3) の場合は true、それ以外の場合は false。
		/// </returns>
		public static bool IsSupported
		{
			get
			{
				var version = Environment.OSVersion.Version;
				return version.Major == 6 && version.Minor == 3;
			}
		}

		/// <summary>
		/// 現在の <see cref="HwndSource"/> が描画されているモニターの DPI 設定値を取得します。
		/// </summary>
		/// <param name="hwndSource">DPI を取得する対象の Win32 ウィンドウを特定する <see cref="HwndSource"/> オブジェクト。</param>
		/// <param name="dpiType">DPI の種類。規定値は <see cref="MonitorDpiType.Default"/> (<see cref="MonitorDpiType.EffectiveDpi"/> と同値) です。</param>
		/// <returns><paramref name="hwndSource"/> が描画されているモニターの DPI 設定値。サポートされていないシステムの場合は <see cref="Dpi.Default"/>。</returns>
		public static Dpi GetDpi(this HwndSource hwndSource, MonitorDpiType dpiType = MonitorDpiType.Default)
		{
			if (!IsSupported) return Dpi.Default;

			var hmonitor = NativeMethods.MonitorFromWindow(
				hwndSource.Handle,
				MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);

			uint dpiX = 1, dpiY = 1;
			NativeMethods.GetDpiForMonitor(hmonitor, dpiType, ref dpiX, ref dpiY);

			return new Dpi(dpiX, dpiY);
		}
	}
}
