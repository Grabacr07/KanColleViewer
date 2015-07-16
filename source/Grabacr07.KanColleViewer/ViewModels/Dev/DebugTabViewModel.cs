using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Dev
{
	public class DebugTabViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Properties.Resources.Debug; }
			protected set { throw new NotImplementedException(); }
		}

		public void Notify()
		{
			PluginService.Current.GetNotifier().Show(
				Properties.Resources.Debug_NotificationMessage_Title,
				Properties.Resources.Debug_NotificationMessage,
				() => WindowService.Current.MainWindow.Activate());
		}

	}
}
