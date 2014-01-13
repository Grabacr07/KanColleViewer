using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// 通知機能をラップします。
	/// </summary>
	public interface INotifyWrapper
	{
		void Initialize();

		void Show(
			string header, string body,
			Action activated, Action<ToastDismissalReason> dismissed = null, Action<Exception> failed = null);
	}
}
