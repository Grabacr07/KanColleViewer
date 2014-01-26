using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Navigation;
using Grabacr07.Desktop.Metro;
using Grabacr07.KanColleViewer.Properties;
using MetroRadiance.Core;
using mshtml;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	internal class SetStyleSheetBehavior : Behavior<WebBrowser>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.LoadCompleted += this.AssociatedObjectOnNavigated;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.AssociatedObject.LoadCompleted -= this.AssociatedObjectOnNavigated;
		}

		private void AssociatedObjectOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
		{
			this.SetStyleSheet();
		}


		private void SetStyleSheet()
		{
			var document = this.AssociatedObject.Document as HTMLDocument;
			if (document == null) return;

			var gameFrame = document.getElementById("game_frame");
			if (gameFrame == null)
			{
				if (document.url.Contains(".swf?"))
					gameFrame = document.body;
				else
					return;
			}

			var target = gameFrame.document as HTMLDocument;
			if (target == null) return;

			target.createStyleSheet().cssText = Properties.Settings.Default.OverrideStyleSheet;

			var dpi = this.AssociatedObject.GetSystemDpi();

			var window = Window.GetWindow(this.AssociatedObject);
			if (window != null && window.WindowState == WindowState.Normal)
			{
				window.SizeToContent = SizeToContent.Width;
			}

			this.AssociatedObject.Width = 800 / dpi.ScaleX;
			this.AssociatedObject.Height = 480 / dpi.ScaleY;
		}
	}
}
