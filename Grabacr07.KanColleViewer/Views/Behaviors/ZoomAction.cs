using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public class ZoomAction : InteractionMessageAction<WebBrowser>
	{
		protected override void InvokeAction(InteractionMessage message)
		{
			var zoomMessage = message as ZoomMessage;
			if (zoomMessage == null) return;

			if (zoomMessage.ZoomFactor < 10 || zoomMessage.ZoomFactor > 1000)
			{
				StatusService.Current.Notify(string.Format(Resources.ZoomAction_OutOfRange, zoomMessage.ZoomFactor));
				return;
			}

			try
			{
				var provider = this.AssociatedObject.Document as IServiceProvider;
				if (provider == null) return;

				object ppvObject;
				provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
				var webBrowser = ppvObject as IWebBrowser2;
				if (webBrowser == null) return;

				object pvaIn = zoomMessage.ZoomFactor;
				webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				StatusService.Current.Notify(string.Format(Resources.ZoomAction_ZoomFailed, ex.Message));
			}
		}
	}
}
