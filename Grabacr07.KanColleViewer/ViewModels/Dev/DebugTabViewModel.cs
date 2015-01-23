using Grabacr07.KanColleViewer.Composition;
using System;

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
			PluginHost.Instance.GetNotifier()
				.Show(NotifyType.Other, Properties.Resources.Debug_NotificationMessage_Title, Properties.Resources.Debug_NotificationMessage, () => App.ViewModelRoot.Activate());
		}
		//public void CriticalDialog()
		//{
		//	//var MsgBox = new CriticalDialogViewModel();
		//	//var message = new TransitionMessage(MsgBox, "Show/CriticalDialog");
		//	//this.Messenger.RaiseAsync(message);
		//	App.Current.Dispatcher.Invoke(
		//					System.Windows.Threading.DispatcherPriority.Normal,
		//					new Action(
		//						delegate()
		//						{
		//							App.CriticalPupup();
		//						})
		//					);
		//}

	}
}
