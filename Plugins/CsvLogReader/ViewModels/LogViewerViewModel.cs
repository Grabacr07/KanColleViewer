using Livet;
using Livet.Messaging;

namespace CsvLogReader.ViewModels
{
	public class LogViewerViewModel : ViewModel
	{
		public LogViewerViewModel()
		{

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
