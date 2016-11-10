using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Interop;
using Windows.UI.Input;
using Grabacr07.KanColleViewer;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.ViewModels;
using Application = System.Windows.Application;

namespace SurfaceDialSupport
{
	[Export(typeof(IPlugin))]
	[ExportMetadata("Guid", "F8BFF484-1894-4881-9720-8907265511FD")]
	[ExportMetadata("Title", "SurfaceDialSupport")]
	[ExportMetadata("Description", "Surface Dial へのサポートを追加します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class DialSupportPlugin : IPlugin
	{
		private bool initialized;
		private RadialController controller;
		private RadialControllerConfiguration configuration;

		public void Initialize()
		{
			Application.Current.Startup += (sender, args) =>
			{
				var window = Application.Current.MainWindow;
				window.SourceInitialized += (s, e) => this.InitializeCore();
				this.InitializeCore();
			};
		}

		private void InitializeCore()
		{
			if (this.initialized) return;

			var window = Application.Current.MainWindow;
			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source == null) return;

			this.controller = DesktopRadialController.Create(source.Handle);
			this.configuration = DesktopRadialControllerConfiguration.Create(source.Handle);

			this.configuration.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, });
			this.controller.Menu.Items.Add(RadialControllerMenuItem.CreateFromKnownIcon("Tab", RadialControllerMenuKnownIcon.Scroll));
			this.controller.RotationChanged += (sender, args) => this.ChangeTab(args.RotationDeltaInDegrees < 0);

			this.initialized = true;
		}

		private void ChangeTab(bool prev)
		{
			var vm = WindowService.Current.Information;
			var oldIndex = vm.TabItems.IndexOf(vm.SelectedItem);
			var newIndex = oldIndex + (prev ? -1 : 1);

			if (newIndex < 0) newIndex = vm.TabItems.Count - 1;
			else if (newIndex >= vm.TabItems.Count) newIndex = 0;

			vm.SelectedItem = vm.TabItems[newIndex];
		}
	}


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
