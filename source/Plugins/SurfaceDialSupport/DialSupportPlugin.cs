using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Windows.UI.Input;

namespace Grabacr07.KanColleViewer.Plugins.DialSupport
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
		private Action[] activateActions;
		private int index;

		public void Initialize()
		{
			System.Windows.Application.Current.Startup += (sender, args) =>
			{
				var window = System.Windows.Application.Current.MainWindow;
				window.SourceInitialized += (s, e) => this.InitializeCore();
				this.InitializeCore();
			};
		}

		private void InitializeCore()
		{
			if (this.initialized) return;

			var window = System.Windows.Application.Current.MainWindow;
			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source == null) return;

			this.controller = DesktopRadialController.Create(source.Handle);
			this.configuration = DesktopRadialControllerConfiguration.Create(source.Handle);

			this.configuration.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, });
			this.controller.Menu.Items.Add(RadialControllerMenuItem.CreateFromKnownIcon("Tab", RadialControllerMenuKnownIcon.Scroll));
			this.controller.RotationChanged += (sender, args) => this.Action(args.RotationDeltaInDegrees < 0);

			this.initialized = true;
		}

		private IEnumerable<Action> CreateActivateActions(InformationViewModel vm)
		{
			foreach (var item in vm.TabItems)
			{
				var tools = item as ToolsViewModel;
				if (tools != null)
				{
					foreach (var tool in tools.Tools)
					{
						yield return () =>
						{
							vm.SelectedItem = item;
							tools.SelectedTool = tool;
						};
					}
					continue;
				}

				yield return () => vm.SelectedItem = item;
			}

			foreach (var item in vm.SystemTabItems)
			{
				yield return () => vm.SelectedItem = item;
			}
		}

		private void Action(bool prev)
		{
			if (this.activateActions == null)
			{
				this.activateActions = this.CreateActivateActions(WindowService.Current.Information).ToArray();
			}

			if (prev) this.index--;
			else this.index++;

			if (this.index < 0) this.index = this.activateActions.Length - 1;
			else if (this.index >= this.activateActions.Length) this.index = 0;

			this.activateActions[this.index]();
		}
	}
}
