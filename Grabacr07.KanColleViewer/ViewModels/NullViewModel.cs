using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public sealed class NullViewModel : ViewModel
	{
		public static NullViewModel Instance { get; private set; }

		static NullViewModel()
		{
			Instance = new NullViewModel();
		}

		private NullViewModel() { }
	}
}
