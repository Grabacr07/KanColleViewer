using Livet;
using Livet.Messaging;

namespace CsvLogReader.ViewModels
{
	public class LogViewerViewModel : ViewModel
	{
		#region static members
		private static readonly LogViewerViewModel current = new LogViewerViewModel();

		public static LogViewerViewModel Current
		{
			get { return current; }
		}
		#endregion
		public LogDataList LogDataList { get; set; }

		public LogViewerViewModel()
		{
			LogDataList = new LogDataList();
		}
		public void ShowDropLogViewer()
		{
			var catalog = new DropLogViewerViewModel();
			var message = new TransitionMessage(catalog, "Show/DropLogViewer");
			this.Messenger.RaiseAsync(message);
		}
		public void ShowShipBuildLogViewer()
		{
			var catalog = new ShipBuildLogViewerViewModel();
			var message = new TransitionMessage(catalog, "Show/ShipBuildLogViewer");
			this.Messenger.RaiseAsync(message);
		}
		public void ShowItemBuildLogViewer()
		{
			var catalog = new ItemBuildLogViewerViewModel();
			var message = new TransitionMessage(catalog, "Show/ItemBuildLogViewer");
			this.Messenger.RaiseAsync(message);
		}
	}
}
