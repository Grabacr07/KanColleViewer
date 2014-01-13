using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// Windows の通知を提供します。
	/// <see cref="T:Grabacr07.KanColleViewer.Model.Toast"/> クラスと
	/// <see cref="T:Grabacr07.KanColleViewer.Model.NotifyIconWrapper"/> クラスの切り替えを隠蔽します。
	/// </summary>
	public static class WindowsNotifier
	{
		public static INotifyWrapper Current { get; set; }

		public static void Initialize()
		{
			if (Toast.IsSupported)
			{
				Current = new ToastHelper();
			}
			else
			{
				Current = new NotifyIconWrapper();
			}

			Current.Initialize();
		}
	}
}
