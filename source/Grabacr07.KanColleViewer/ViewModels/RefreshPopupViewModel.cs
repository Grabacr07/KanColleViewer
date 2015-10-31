
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class RefreshPopupViewModel : WindowViewModel
	{
		public RefreshPopupViewModel()
		{
			
		}
		public void RefreshNav()
		{
			WindowService.Current.RefreshWindow();
			this.Close();
		}
		public void PopupClose()
		{
			this.Close();
		}
	}
}
