using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Input;

namespace Grabacr07.KanColleViewer.Plugins.DialSupport
{
	public static class DesktopRadialController
	{
		public static RadialController Create(IntPtr hWnd)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			var controller = (IDesktopRadialController)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
			var iid = typeof(RadialController).GetInterface("IRadialController").GUID;

			return controller.CreateForWindow(hWnd, ref iid);
		}
	}

	public static class DesktopRadialControllerConfiguration
	{
		public static RadialControllerConfiguration Create(IntPtr hWnd)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			var configration = (IDesktopRadialControllerConfiguration)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialControllerConfiguration));
			var iid = typeof(RadialControllerConfiguration).GetInterface("IRadialControllerConfiguration").GUID;

			return configration.GetForWindow(hWnd, ref iid);
		}
	}

	[Guid("1b0535c9-57ad-45c1-9d79-ad5c34360513")]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	public interface IDesktopRadialController
	{
		RadialController CreateForWindow(IntPtr hWnd, [In] ref Guid iid);
	}

	[Guid("787cdaac-3186-476d-87e4-b9374a7b9970")]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	public interface IDesktopRadialControllerConfiguration
	{
		RadialControllerConfiguration GetForWindow(IntPtr hWnd, [In] ref Guid iid);
	}
}
